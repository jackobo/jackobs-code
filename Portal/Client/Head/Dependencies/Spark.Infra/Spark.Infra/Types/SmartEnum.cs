using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace Spark.Infra.Types
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
            var result = TryFindFromId(id);
            if (result.Any())
                return result.First();
            else
                throw new ArgumentException($"Can't find an item with id '{id.ToString()}'");
            
        }

        public static Optional<TConcreteClass> TryFindFromId(TID id)
        {
            if (LookupItemsById.ContainsKey(id))
                return Optional<TConcreteClass>.Some(LookupItemsById[id]);
            else
                return Optional<TConcreteClass>.None();
        }


        public static TConcreteClass FromName(string name)
        {
            var result = TryFindFromName(name);
            if (result.Any())
                return result.First();
            else
                throw new ArgumentException($"Can't find an an item with name '{name}'");
        }


        public static Optional<TConcreteClass> TryFindFromName(string name)
        {
            if (LookupItemsByName.ContainsKey(name))
                return Optional<TConcreteClass>.Some(LookupItemsByName[name]);
            else
                return Optional<TConcreteClass>.None();
        }

    }
}
