using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GamesPortal.Client.Interfaces
{
    public abstract class SmartEnum<TID, TConcreteClass> where TConcreteClass : SmartEnum<TID, TConcreteClass>
    {

        protected SmartEnum(TID id, string name)
        {
            Id = id;
            Name = name;
        }


        public TID Id { get; private set; }
        public string Name { get; private set; }


        public override bool Equals(object obj)
        {
            var theOther = obj as TConcreteClass;

            if (object.ReferenceEquals(theOther, null))
                return false;

            return Id.Equals(theOther.Id);
        }


        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }




        public static bool operator ==(SmartEnum<TID, TConcreteClass> v1, SmartEnum<TID, TConcreteClass> v2)
        {
            if (!object.ReferenceEquals(v1, null))
                return v1.Equals(v2);
            else if (!object.ReferenceEquals(v2, null))
                return v2.Equals(v1);
            return true;

        }

        public static bool operator !=(SmartEnum<TID, TConcreteClass> v1, SmartEnum<TID, TConcreteClass> v2)
        {
            return !(v1 == v2);
        }


        private static object _lookupItemsByIdSync = new object();

        private static Dictionary<TID, TConcreteClass> _lookupItemsById;

        private static Dictionary<TID, TConcreteClass> LookupItemsById
        {
            get
            {
                if (_lookupItemsById == null)
                {
                    lock (_lookupItemsByIdSync)
                    {
                        if (_lookupItemsById == null)
                        {
                            _lookupItemsById = All.ToDictionary(ct => ct.Id);
                        }
                    }
                }
                return _lookupItemsById;
            }
        }

        private static object _lookupItemsByNameSync = new object();

        private static Dictionary<string, TConcreteClass> _lookupItemsByName;

        public static Dictionary<string, TConcreteClass> LookupItemsByName
        {
            get
            {
                if (_lookupItemsByName == null)
                {
                    lock (_lookupItemsByNameSync)
                    {
                        if (_lookupItemsByName == null)
                        {
                            _lookupItemsByName = All.ToDictionary(t => t.Name);
                        }
                    }
                }
                return _lookupItemsByName;
            }

        }

        private static object _listSync = new object();
        private static TConcreteClass[] _list = null;

        public static TConcreteClass[] All
        {
            get
            {
                if (_list == null)
                {
                    lock (_listSync)
                    {
                        if (_list == null)
                        {
                            _list = typeof(TConcreteClass).GetFields(BindingFlags.Static | BindingFlags.Public)
                                                         .Select(fieldInfo => (TConcreteClass)fieldInfo.GetValue(null))
                                                         .ToArray();
                        }
                    }
                }
                return _list;
            }
        }

        public static TConcreteClass FromId(TID id)
        {
            return LookupItemsById[id];
        }

        public static TConcreteClass FromIdOrNull(TID id)
        {
            if (LookupItemsById.ContainsKey(id))
                return LookupItemsById[id];
            else
                return null;
        }


        public static TConcreteClass FromName(string name)
        {
            return LookupItemsByName[name];
        }


        public static TConcreteClass FromNameOrNull(string name)
        {
            if (LookupItemsByName.ContainsKey(name))
                return LookupItemsByName[name];
            else
                return null;
        }

    }
}
