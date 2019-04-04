using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutTool.ViewModels
{
    public class ArenaGameViewModel : DragableDropableViewModel, ILobbyItemSource, ICustomTypeDescriptor, IConvertible<AvailableGameViewModel>
    {
        public ArenaGameViewModel(AvailableGameViewModel availableGame, ArenaGameCollectionViewModel parentCollection)
        {
            if (availableGame == null)
                throw new ArgumentNullException(nameof(availableGame));

            if (parentCollection == null)
                throw new ArgumentNullException(nameof(parentCollection));

            _availableGame = availableGame;
            _parentCollection = parentCollection;
            _parentCollection.CollectionChanged += ParentCollection_CollectionChanged;
            _availableGame.PropertyChanged += AvailableGame_PropertyChanged;
            
            
        }

        AvailableGameViewModel _availableGame;
        ArenaGameCollectionViewModel _parentCollection;

        private static PropertyDescriptor[] _availableGamePropertyDescriptors;
        
        static ArenaGameViewModel()
        {
            CreatePropertyTypeDescriptors();
        }

        private static void CreatePropertyTypeDescriptors()
        {
            var propDescriptors = new List<PropertyDescriptor>();
            foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(typeof(AvailableGameViewModel)))
            {
                propDescriptors.Add(new AvailableGamePropertyDescriptor(pd));
            }

            _availableGamePropertyDescriptors = propDescriptors.ToArray();
        }

        public AvailableGameViewModel ConvertToAvailableGame()
        {
            return _availableGame;
        }

        private void ParentCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(PositionInsideThePage));
            OnPropertyChanged(nameof(PositionInsideTheArena));
        }
        

        public int PositionInsideThePage
        {
            get
            {
                var index = (_parentCollection.IndexOf(this) + 1) % Constants.ArenaPageSize;
                if (index == 0)
                    return Constants.ArenaPageSize;
                else
                    return index;
            }
        }

        public int PositionInsideTheArena
        {
            get { return _parentCollection.IndexOf(this) + 1; }
        }

        private void AvailableGame_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }
        

        

        int ILobbyItemSource.Id
        {
            get
            {
                return ((ILobbyItemSource)_availableGame).Id;
            }
        }

        string ILobbyItemSource.Name
        {
            get
            {
                return ((ILobbyItemSource)_availableGame).Name;
            }
        }

        bool ILobbyItemSource.ShouldShowTheJackpot(PlayerStatusTypeViewModel playerStatus)
        {
            return ((ILobbyItemSource)_availableGame).ShouldShowTheJackpot(playerStatus);
        }

        protected override bool CanDropItem(object droppedItem, DropContext context)
        {
            if (base.CanDropItem(droppedItem, context))
                return true;

            return droppedItem is AvailableGameViewModel;
        }

        protected override object DropItem(object droppedItem, DropContext context)
        {
            if (droppedItem is AvailableGameViewModel)
            {
                droppedItem = new ArenaGameViewModel(droppedItem as AvailableGameViewModel, _parentCollection);
            }

            return base.DropItem(droppedItem, context);
        }


        public override bool Equals(object obj)
        {
            var theOther = obj as ArenaGameViewModel;
            if (theOther == null)
                return false;

            return _availableGame.Equals(theOther._availableGame);
        }

        public override int GetHashCode()
        {
            return _availableGame.GetHashCode();
        }

        public override string ToString()
        {
            return _availableGame.ToString();
        }

        #region ICustomTypeDescriptor implementation
        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return new AttributeCollection();
        }

        string ICustomTypeDescriptor.GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true); ;
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return new PropertyDescriptorCollection(_availableGamePropertyDescriptors);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            return new PropertyDescriptorCollection(_availableGamePropertyDescriptors);
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        AvailableGameViewModel IConvertible<AvailableGameViewModel>.Convert()
        {
            return ConvertToAvailableGame();
        }

        private class AvailableGamePropertyDescriptor : PropertyDescriptor
        {
            PropertyDescriptor _originalPropertyDescriptor;
            public AvailableGamePropertyDescriptor(PropertyDescriptor originalPropertyDescriptor)
                : base(originalPropertyDescriptor)
            {
                _originalPropertyDescriptor = originalPropertyDescriptor;
            }
            public override Type ComponentType
            {
                get
                {
                    return typeof(ArenaGameViewModel);
                }
            }

            public override bool IsReadOnly
            {
                get
                {
                    return _originalPropertyDescriptor.IsReadOnly;
                }
            }

            public override Type PropertyType
            {
                get
                {
                    return _originalPropertyDescriptor.PropertyType;
                }
            }

            public override bool CanResetValue(object component)
            {
                return _originalPropertyDescriptor.CanResetValue(ExtractAvailableGameViewModel(component));
            }

            private AvailableGameViewModel ExtractAvailableGameViewModel(object component)
            {
                return ((ArenaGameViewModel)component).ConvertToAvailableGame();
            }

            public override object GetValue(object component)
            {
                return _originalPropertyDescriptor.GetValue(ExtractAvailableGameViewModel(component));
            }

            public override void ResetValue(object component)
            {
                _originalPropertyDescriptor.ResetValue(ExtractAvailableGameViewModel(component));
            }

            public override void SetValue(object component, object value)
            {
                _originalPropertyDescriptor.SetValue(ExtractAvailableGameViewModel(component), value);
            }

            public override bool ShouldSerializeValue(object component)
            {
                return _originalPropertyDescriptor.ShouldSerializeValue(ExtractAvailableGameViewModel(component));
            }
        }

        #endregion
    }
}
