using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces.Entities;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels.DynamicLayout.ValueEditors
{
    public class ExceededRoundsPlayedValueEditor : SingleValueEditor<int>, IConditionValueEditor
    {
        public ExceededRoundsPlayedValueEditor()
        {

        }

        public ExceededRoundsPlayedValueEditor(int numberOfRounds)
            : base(numberOfRounds)
        {

        }
        

        public int? NumberOfRounds
        {
            get
            {
                return this.Value;
            }

            set
            {
                this.Value = value;
                OnPropertyChanged();

            }
        }

        private int? _gameType;
        public int? GameType
        {
            get
            {
                return _gameType;
            }

            set
            {
                SetProperty(ref _gameType, value);
            }
        }

        

        public override IConditionValueEditor Clone()
        {
            return new ExceededRoundsPlayedValueEditor() { GameType = this.GameType, Value = this.Value };
        }

        protected override int Incrementer(int value)
        {
            return value + 1;
        }

        protected override int Decrementer(int value)
        {
            return value - 1;
        }

        protected override string FormatValue(int value)
        {
            return value.ToString();
        }

    }
}
