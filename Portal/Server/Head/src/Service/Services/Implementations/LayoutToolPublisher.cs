using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Configurations;
using Spark.Infra.Types;
using Microsoft.Practices.Unity;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using GamesPortal.Service.Configurations;
using System.Xml.Linq;

namespace GamesPortal.Service
{
    public class LayoutToolPublisher : WcfServiceBase, ILayoutToolPublisher
    {
        public LayoutToolPublisher(IGamesPortalInternalServices services)
            : base(services)
        {
            
        }

        public void PublishSkinForQA(PublishSkinToQARequest request)
        {
            try
            {
                var result = UploadToTfs(request.SkinContent, 
                                        $"New skin added for QA. Environment = {request.Environment} Brand = {request.BrandId} skin = {request.SkinId}; Published by {Services.CallContextServices.GetCallingUserName()}",
                                        null,
                                        "QA", 
                                        request.Environment,
                                        $"Brand_{request.BrandId}", 
                                        $"Skin_{request.SkinId}");
                
                SendNotification(result.ServerFileFullPath, request.BrandId, request.SkinId, request.ClientVersion, request.Environment, request.HasWarnings);

            }
            catch (Exception ex)
            {
                LogException(nameof(PublishSkinForQA), ex);
                throw;
            }
        }
        
        public void PublishSkinForProduction(PublishSkinToProductionRequest request)
        {
            try
            {
                var result = UploadToTfs(request.SkinContent,
                                        $"New skin added for PRODUCTION. Brand = {request.BrandId} Skin = {request.SkinId}; Published by {Services.CallContextServices.GetCallingUserName()}",
                                        request.NavigationPlanContent,
                                        "Production",
                                        $"Brand_{request.BrandId}",
                                        $"Skin_{request.SkinId}");

                
                
                SendNotification(result.ServerFileFullPath, request.BrandId, request.SkinId, request.ClientVersion, "Production", request.HasWarnings);

            }
            catch (Exception ex)
            {
                LogException(nameof(PublishSkinForProduction), ex);
                throw;
            }
        }

        private void SendNotification(string serverSkinFile, int brandId, int skinId, string clientVersion, string environmentName, bool hasWarnings)
        {
            var cc = new List<string>();
            
            var userName = Services.CallContextServices.GetCallingUserName();
            var userDetails = Services.CallContextServices.GetCallingUserDetails();
            var publishingUserEmailAddress = userDetails.FirstOrDefault()?.EmailAddress;
            var publishingUserDisplayName = userDetails.FirstOrDefault()?.DisplayName ?? userName;

            if (string.IsNullOrEmpty(publishingUserEmailAddress))
            {
                Logger.Warning($"Can't find e-mail address for user {publishingUserDisplayName}");
            }
            else
            {
                cc.Add(userDetails.First().EmailAddress);
                
            }

            var to = (ConfigurationReader.ReadSection<LayoutToolPublisherSettings>().MailingList ?? string.Empty)
                     .Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                     .Select(email => email.Trim())
                     .ToList();

            
            if (to.Count == 0)
            {
                Logger.Error("There is no mailingList configuration for LayoutToolPublisher");

                if(cc.Count > 0)
                {
                    string subject = $"Layout publishing notification failed!";
                    string body = $"The layout for brand {brandId}, skin {skinId} and {environmentName} environment was published in TFS here {serverSkinFile}, but the system failed to notify the DevOps team!{Environment.NewLine}Please contact the DevOps team and report this error!";
                    SendEmail(to, cc, subject, body);
                }
            }
            else
            {
                string subject = $"New layout available for brand {brandId} skin {skinId} in {environmentName} environment for client version {clientVersion}";
                var body = new StringBuilder();
                body.AppendLine($"A new layout was published by {publishingUserDisplayName} for brand {brandId}, skin {skinId} and {environmentName} environment.");
                body.AppendLine($"You can find this new layout file in TFS here {serverSkinFile}");
                if (hasWarnings)
                {
                    body.AppendLine();
                    body.AppendLine("ATENTION!!!!!");
                    body.AppendLine("There are some warnings in the layout file generated by the Layout Tool. Please check the warnings before using this file!");
                }

                SendEmail(to, cc, subject, body.ToString());
            }
            

        }

        private void SendEmail(IEnumerable<string> to, IEnumerable<string> cc, string subject, string body)
        {
            var currentUserEmailAddress = System.DirectoryServices.AccountManagement.UserPrincipal.Current.EmailAddress;
            using (var msg = new System.Net.Mail.MailMessage())
            {
                msg.From = new System.Net.Mail.MailAddress(currentUserEmailAddress, Environment.UserName, System.Text.Encoding.UTF8);
                
                foreach(var addr in to)
                    msg.To.Add(addr);

                foreach (var addr in cc)
                    msg.CC.Add(addr);

                msg.Subject = subject;
                msg.Body = body;

                using (System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient())
                {
                    client.UseDefaultCredentials = true;
                    client.EnableSsl = true;
                    client.Port = 587;

                    client.Host = "xch-il.888holdings.corp";

                    client.Send(msg);
                }
            }
        }

        private UploadTextFileContentResult UploadToTfs(string skinContent, string tfsComment, string navigationPlanContent, params string[] relativePathToWorkingFolder)
        {
            var workspace = Services.TfsGateway.GetLayoutToolWorkspace();

            
            var result = UploadFileContentToTfs(skinContent, 
                                                tfsComment, 
                                                workspace,
                                                relativePathToWorkingFolder.Concat(new string[] { GetLayoutToolFileName() }).ToArray());

            if(string.IsNullOrEmpty(navigationPlanContent))
            {
                return result;
            }

            result = UploadFileContentToTfs(navigationPlanContent,
                                            tfsComment,
                                            workspace,
                                            relativePathToWorkingFolder.Concat(new string[] { GetNavigationPlanFileName() }).ToArray());

            return result;
        }

        private UploadTextFileContentResult UploadFileContentToTfs(string fileContent, string tfsComment, ITfsWorkspace workspace, string[] fileRelativePathToWorkingFolder)
        {
            var result = workspace.UploadTextFileContent(fileContent, tfsComment, fileRelativePathToWorkingFolder);

            try
            {
                Services.FileSystemManager.DeleteFile(result.LocalFileFullPath);
            }
            catch (Exception ex)
            {
                LogException($"UploadToTfs:  Delete local file {result.LocalFileFullPath} failed!", ex);
            }

            return result;
        }

        private string GetLayoutToolFileName()
        {
            var dateTime = DateTime.Now;

            return $"{dateTime.Year.ToString()}-{dateTime.Month.ToString().PadLeft(2, '0')}-{dateTime.Day.ToString().PadLeft(2, '0')}-{dateTime.Hour.ToString().PadLeft(2, '0')}-{dateTime.Minute.ToString().PadLeft(2, '0')}-{dateTime.Second.ToString().PadLeft(2, '0')}"
                    + ".lyt";
        }

        private string GetNavigationPlanFileName()
        {
            var dateTime = DateTime.Now;

            return $"{dateTime.Year.ToString()}-{dateTime.Month.ToString().PadLeft(2, '0')}-{dateTime.Day.ToString().PadLeft(2, '0')}-{dateTime.Hour.ToString().PadLeft(2, '0')}-{dateTime.Minute.ToString().PadLeft(2, '0')}-{dateTime.Second.ToString().PadLeft(2, '0')}"
                    + ".xmm";
        }

        public GetCurrentProductionNavigationPlanResponse GetCurrentProductionNavigationPlan(GetCurrentProductionNavigationPlanRequest request)
        {

            try
            {
                var changeSet = GetNavigationPlanChangeSet(request);
                if(changeSet == null)
                {
                    Logger.Warning($"Can't get the changeset from Jenkins for Brand = {request.BrandId}; Skin = {request.SkinId}; ClientVersion = {request.ClientVersion}; JobNumber={request.ClientVersionJobNumber}");
                    return new GetCurrentProductionNavigationPlanResponse();
                }

                string filePath = "";
                if(Services.TfsGateway.FolderExists($"$/GamingNDL/Develop/CasinoFlashClient/Configuration/Branches/{request.ClientVersion}"))
                {
                    filePath = $"$/GamingNDL/Develop/CasinoFlashClient/Configuration/Branches/{request.ClientVersion}/navigation/Brand_{request.BrandId}/skin_{request.SkinId}/navigation_plan_ndl.xmm";
                }
                else
                {
                    filePath = $"$/GamingNDL/Develop/CasinoFlashClient/Configuration/HEAD/navigation/Brand_{request.BrandId}/skin_{request.SkinId}/navigation_plan_ndl.xmm";
                }
                
                var fileContent = Services.TfsGateway.ReadFileContent(filePath, changeSet.Value);
                if (fileContent.Any())
                    return new GetCurrentProductionNavigationPlanResponse(fileContent.First());
                else
                    return new GetCurrentProductionNavigationPlanResponse();
            }
            catch(Exception ex)
            {
                Logger.Exception(ex);
                return new GetCurrentProductionNavigationPlanResponse();
            }
        }

        private int? GetNavigationPlanChangeSet(GetCurrentProductionNavigationPlanRequest request)
        {
            var restClient = new RestSharp.RestClient("http://jenkinsprod:8080/");
            restClient.Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator("Smaster", "MasterS1");
            var response = restClient.Execute(new RestSharp.RestRequest($"job/Casino/job/CCK/job/{request.ClientVersion}/job/BuildConfiguration/{request.ClientVersionJobNumber}/api/xml?xpath=/multiJobBuild/action/changesetVersion&wrapper=multiJobBuild", RestSharp.Method.GET));
            if(string.IsNullOrEmpty(response.Content))
            {
                return null;
            }

            var xmlDoc = XDocument.Parse(response.Content);
            return int.Parse(xmlDoc.Root.Elements("changesetVersion").Skip(1).First().Value);
        }
    }
}
