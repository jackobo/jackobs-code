using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Web.Administration;

namespace GGPMockBootstrapper.Models.Client
{
    public class Html5ClientInstallAction : IISAppInstallActionBase<ClientProduct>
    {
        public Html5ClientInstallAction(ClientProduct clientProduct)
            : base(clientProduct, AppName, Microsoft.Web.Administration.ManagedPipelineMode.Integrated)
        {
        }


        public static readonly string AppName = "Html5GamesForGGPMock";

        public override string Description
        {
            get
            {
                return "Install Sample HTML5 games for GGPMock";
            }
        }

        protected override string GetPackageName()
        {
            return "Html5Games.zip";
        }

        protected override void ConfigureApplicationCore(IInstalationContext context, ServerManager iisManager)
        {
            base.ConfigureApplicationCore(context, iisManager);

            AddMimeTypes(iisManager,
                         new MimeTypeDefinition(".mp3", "audio/mpeg"),
                         new MimeTypeDefinition(".ogg", "audio/ogg"),
                         new MimeTypeDefinition(".m4a", "audio/mp4"),

                         new MimeTypeDefinition(".mp4", "video/mpeg"),
                         new MimeTypeDefinition(".webm", "video/webm"),

                         new MimeTypeDefinition(".fnt", "text/xml"),
                         new MimeTypeDefinition(".eof", "application/octet-stream"),
                         new MimeTypeDefinition(".ttf", "application/octet-stream"),
                         new MimeTypeDefinition(".otf", "application/x-font-opentype"),
                         new MimeTypeDefinition(".woff", "application/font-woff"),
                         new MimeTypeDefinition(".woff2", "application/font-woff2"),
                         new MimeTypeDefinition(".eot", "application/vnd.ms-fontobject"),
                         new MimeTypeDefinition(".sfnt", "application/font-sfnt"),
                         new MimeTypeDefinition(".atlas", "application/json"));
        }
    }
}
