using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Spark.Infra.Types
{
    public class VersionNumber : ICloneable, IComparable, IComparable<VersionNumber>, IEquatable<VersionNumber>, IXmlSerializable
    {
        public VersionNumber()
        {
            
        }

        public VersionNumber(string version)
        {
            var v = Parse(version);
            this.Major = v.Major;
            this.Minor = v.Minor;
            this.Revision = v.Revision;
            this.Build = v.Build;
            _parsedVersion = version;
        }

        public VersionNumber(long version)
        {
            if (version < 0)
                throw new ArgumentException(string.Format("Negative version ({0}) value is not accepted!", version));

            FromDecimal(version);
        }

        string _parsedVersion = null;
        public string ParsedVersion
        {
            get
            {
                if (string.IsNullOrEmpty(_parsedVersion))
                    return this.ToString();
                else
                    return _parsedVersion;

            }
        }

        public VersionNumber(int major, int minor, int revision, int build)
        {
            if (minor < 0)
                throw new ArgumentException("minor argument cannot be less than zero");

            if (revision < 0)
                throw new ArgumentException("revision argument cannot be less than zero");

            if (build < 0)
                throw new ArgumentException("build argument cannot be less than zero");

            this.Major = major;
            
            this.Minor = minor;
            this.Revision = revision;
            this.Build = build;
        }

        public int Major { get; set; }
        public int Minor { get; set; }
        public int Revision { get; set; }
        public int Build { get; set; }


        public override bool Equals(object obj)
        {
            var theOther = obj as VersionNumber;

            if (theOther == null)
                return false;

            return this.Major == theOther.Major
                    && this.Minor == theOther.Minor
                    && this.Revision == theOther.Revision
                    && this.Build == theOther.Build;

        }

        public override int GetHashCode()
        {
            return this.Major.GetHashCode() ^ this.Minor.GetHashCode() ^ this.Revision.GetHashCode() ^ this.Build.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}.{2}.{3}", this.Major.ToString(), this.Minor.ToString(), this.Revision.ToString(), this.Build.ToString());
        }


        public string ToSortableString()
        {
            return string.Format("{0}.{1}.{2}.{3}",
                                    PaddLeftAndKeepSign(this.Major),
                                    PaddLeftAndKeepSign(this.Minor),
                                    PaddLeftAndKeepSign(this.Revision),
                                    PaddLeftAndKeepSign(this.Build));
        }

        private string PaddLeftAndKeepSign(int value)
        {
            var paddedAbsoluteValue = Math.Abs(value).ToString().PadLeft(4, '0');
            if (value < 0)
                return "_" + paddedAbsoluteValue;
            else
                return paddedAbsoluteValue;
        }



        public static bool operator ==(VersionNumber v1, VersionNumber v2)
        {
            if (!object.ReferenceEquals(v1, null))
                return v1.Equals(v2);
            else if (!object.ReferenceEquals(v2, null))
                return v2.Equals(v1);
            return true;

        }

        public static bool operator !=(VersionNumber v1, VersionNumber v2)
        {
            return !(v1 == v2);
        }

        public static bool operator <(VersionNumber v1, VersionNumber v2)
        {
            if (v1 == null && v2 == null)
                return false;

            if (v1 == null)
                return true;

            if (v2 == null)
                return false;
            
            return ToBigNumber(v1) < ToBigNumber(v2);

        }

        public static bool operator >(VersionNumber v1, VersionNumber v2)
        {
            if (v1 == null && v2 == null)
                return false;

            if (v1 == null)
                return false;

            if (v2 == null)
                return true;


            return ToBigNumber(v1) > ToBigNumber(v2);
        }

        public static VersionNumber operator++(VersionNumber v)
        {
            if (v == null)
                throw new ArgumentNullException();

            return v + 1;
        }

        public static VersionNumber operator +(VersionNumber v, int buildIncrement)
        {
            if (v == null)
                throw new ArgumentNullException();

            return new VersionNumber(v.Major, v.Minor, v.Revision, v.Build + buildIncrement);
        }

        public static bool operator <=(VersionNumber v1, VersionNumber v2)
        {

            if (v1 == v2)
                return true;

            return v1 < v2;
        }


        public static bool operator >=(VersionNumber v1, VersionNumber v2)
        {
            if (v1 == v2)
                return true;

            return v1 > v2;
        }

        

        public static VersionNumber Parse(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            var versionComponents =  text.Replace("_", "-").Split('.');

            if (versionComponents.Length > 4)
                versionComponents = versionComponents.Take(4).ToArray();
                //throw new ArgumentException("Invalid version");

            int major = int.Parse(versionComponents[0]);
            
            int minor = 0;
            if(versionComponents.Length >= 2)
                minor = int.Parse(versionComponents[1]);

            int revision = 0;
            if(versionComponents.Length >= 3)
                revision = int.Parse(versionComponents[2]);

            int build = 0;
            if(versionComponents.Length >= 4)
                build = int.Parse(versionComponents[3]);

            
            return new VersionNumber(major, minor, revision, build);
        }


        public static VersionNumber FromLong(long? version)
        {
            if (version == null)
                return null;

            return new VersionNumber(version.Value);
        }

        private static long ToBigNumber(VersionNumber version)
        {
            return long.Parse(version.ToSortableString().Replace(".", "").Replace("_", "-"));
        }

        public long ToLong()
        {
            long mask = 10000;

            long result = this.Major;
            result *= mask;
            result += this.Minor;
            result *= mask;
            result += this.Revision;
            result *= mask;
            result += this.Build;

            return result;

        }

        private void FromDecimal(decimal version)
        {

            decimal remainingVersion;

            this.Build = GetVersionComponent(version, out remainingVersion);
            this.Revision = GetVersionComponent(remainingVersion, out remainingVersion);
            this.Minor = GetVersionComponent(remainingVersion, out remainingVersion);
            this.Major = GetVersionComponent(remainingVersion, out remainingVersion);
        }

        private int GetVersionComponent(decimal version, out decimal remainingVersion)
        {
            decimal mask = 10000m;
            decimal result = version / mask;
            decimal rest = result - Math.Truncate(result);
            
            remainingVersion = result - rest;

            return (int)(rest * mask);

        }


        #region ICloneable Members

        public object Clone()
        {
            return new VersionNumber(this.Major, this.Minor, this.Revision, this.Build);
        }

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            return CompareTo(obj as VersionNumber);
        }

        #endregion

        #region IComparable<VersionNumber> Members

        public int CompareTo(VersionNumber other)
        {
            
            if (this < other)
                return -1;

            if (this == other)
                return 0;

            return 1;
        }

        #endregion

        #region IEquatable<VersionNumber> Members

        bool IEquatable<VersionNumber>.Equals(VersionNumber other)
        {
            return this.Equals(other);
        }

        #endregion

        #region IXmlSerializable Members

        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
        {
            throw new NotImplementedException();
        }

        void IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
        {
            var version = VersionNumber.Parse(reader.ReadElementContentAsString());
            this.Major = version.Major;
            this.Minor = version.Minor;
            this.Revision = version.Revision;
            this.Build = version.Build;
        }

        void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteValue(this.ToString());
        }

        #endregion
    }
}
