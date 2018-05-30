using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutTool.Interfaces
{
    public sealed class PathDescriptor
    {

        private string[] BasePathComponents = new string[0];
        private string[] Arguments = new string[0];


        private static readonly char SLASH = '/';
        private static readonly char BACK_SLASH = '\\';
        private static readonly char[] SEPARATORS = new char[] { SLASH, BACK_SLASH };
        
        private static readonly char QUESTION = '?';
        private static readonly char AMPERSAND = '&';

        private static readonly string DOT_DOT = "..";



        public PathDescriptor(string path)
        {
            if (string.IsNullOrEmpty(path?.Trim()))
                BasePathComponents = new string[0];
            else
            {
                var components = TrimSeparators(path).Split(QUESTION);
                
                BasePathComponents = components[0].Split(SEPARATORS);
                
                if (components.Length > 1)
                    Arguments = components[1].Split(AMPERSAND);

            }
        }

        private string TrimSeparators(string path)
        {
            if (SEPARATORS.Any(s => path.StartsWith(s.ToString())))
            {
                path = path.Substring(1);
            }

            if (SEPARATORS.Any(s => path.EndsWith(s.ToString())))
            {
                path = path.Substring(0, path.Length - 1);
            }

            return path;
        }

        private PathDescriptor(string[] pathComponents, string[] arguments)
        {
            BasePathComponents = pathComponents ?? new string[0];
            Arguments = arguments ?? new string[0];
        }
        

        public override bool Equals(object obj)
        {
            var theOther = obj as PathDescriptor;
            if (object.ReferenceEquals(theOther, null))
                return false;


            return 0 == string.Compare(this.GetAsNormalizedString(),
                                       theOther.GetAsNormalizedString(),
                                       true);
            
        }

        private string _normalizedString = null;
        private string GetAsNormalizedString()
        {
            if(_normalizedString == null)
            {
                _normalizedString = BuildFullPath(string.Empty);
            }

            return _normalizedString;
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(GetAsNormalizedString());
        }

        public override string ToString()
        {
            return ToHttpUrlFormat();
        }


        public string ToFileSystemFormat()
        {
            return BuildFullPath(BACK_SLASH.ToString());
        }

        public string ToHttpUrlFormat()
        {
            return ReplaceHttpsWithHttp().ToUrlFormat();
        }

        private string ToUrlFormat()
        {
            return BuildFullPath(SLASH.ToString());
        }
        
        
        private string BuildFullPath(string separator)
        {
            string basePath = ResolveRelativePaths(separator);
            
            if (Arguments.Length == 0)
                return basePath;

            string arguments = string.Join(AMPERSAND.ToString(), this.Arguments);
            return $"{basePath}{QUESTION}{arguments}";
        }

        private string ResolveRelativePaths(string separator)
        {
            if (this.BasePathComponents.Length == 0)
                return string.Empty;

            var result = new List<string>();

            int i = 0;

            while(BasePathComponents[i] == DOT_DOT)
            {
                result.Add(BasePathComponents[i]);
                i++;
            }
            
            result.AddRange(BasePathComponents.Skip(i).Where(s => s != DOT_DOT));

            return string.Join(separator, result);
        }


        public override int GetHashCode()
        {
            return this.GetAsNormalizedString().GetHashCode();
        }

        public static bool operator ==(PathDescriptor p1, PathDescriptor p2)
        {
            if (!object.ReferenceEquals(p1, null))
                return p1.Equals(p2);
            else if (!object.ReferenceEquals(p2, null))
                return p2.Equals(p1);

            return true;

        }

        public static bool operator !=(PathDescriptor p1, PathDescriptor p2)
        {
            return !(p1 == p2);
        }

        public PathDescriptor Replace(string search, string replaceWith)
        {
            return new PathDescriptor(this.BasePathComponents.Select(c => c.Replace(search, replaceWith)).ToArray(),
                                      this.Arguments.Select(c => c.Replace(search, replaceWith)).ToArray());
        }

        public bool EndsWith(PathDescriptor path)
        {
            if (BasePathComponents.Length < path.BasePathComponents.Length)
                return false;

            var k = this.BasePathComponents.Length - 1;
            for(int i = path.BasePathComponents.Length - 1; i >= 0; i--)
            {
                if (0 != string.Compare(path.BasePathComponents[i], this.BasePathComponents[k], true))
                    return false;
                k--;
            }

            return true;
        }
        
        public static PathDescriptor operator +(PathDescriptor p1, PathDescriptor p2)
        {
            if (object.ReferenceEquals(p1, null))
                return p2;

            if (object.ReferenceEquals(p2, null))
                return p1;

            
            return new PathDescriptor(p1.BasePathComponents.Concat(p2.BasePathComponents).ToArray(),
                                      p1.Arguments.Concat(p2.Arguments).ToArray());

        }

        private PathDescriptor ReplaceHttpsWithHttp()
        {
            if (this.IsEmpty())
                return this;


            if (BasePathComponents[0] != "https:")
            {
                return this;
            }

            var result = new PathDescriptor(BasePathComponents.ToArray(), Arguments.ToArray());
            result.BasePathComponents[0] = "http:";
            return result;
        }
    }
}
