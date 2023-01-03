using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Datatype;
using Tekla.Structures.Dialog;
using TD = Tekla.Structures.Datatype;

namespace StudArrayPlugin
{
    public class MainWindowViewModel : BaseViewModel
    {
        [StructuresDialog("Profile", typeof(TD.String))]
        public string Profile { get; set; } = "Шпилька SD1-19x150-A";

        [StructuresDialog("Material", typeof(TD.String))]
        public string Material { get; set; } = "Steel_Undefined";

        [StructuresDialog("StudClass", typeof(TD.String))]
        public string StudClass { get; set; } = "2";

        [StructuresDialog("StudPrefix", typeof(TD.String))]
        public string StudPrefix { get; set; } = "515-6.";

        [StructuresDialog("StudHeight", typeof(TD.Double))]
        public double StudHeight { get; set; } = 150.0;

        [StructuresDialog("StudCrossStep", typeof(TD.Double))]
        public double StudCrossStep { get; set; } = 100.0;

        [StructuresDialog("StudAlongStep", typeof(TD.Double))]
        public double StudAlongStep { get; set; } = 300.0;

        [StructuresDialog("StudCrossNum", typeof(TD.Integer))]
        public int StudCrossNum { get; set; } = 2;

        [StructuresDialog("StudAlongNum", typeof(TD.Integer))]
        public int StudAlongNum { get; set; } = 10;

        [StructuresDialog("StudOffset", typeof(TD.Double))]
        public double StudOffset { get; set; } = 300.0;

        [StructuresDialog("StudLastNum", typeof(TD.Integer))]
        public int StudLastNum { get; set; } = 0;

        [StructuresDialog("StudLastStep", typeof(TD.Double))]
        public double StudLastStep { get; set; } = 100.0;

        [StructuresDialog("StudCreateBlue", typeof(TD.Integer))]
        public int StudCreateBlue { get; set; } = 1;

        [StructuresDialog("StudCreateGreen", typeof(TD.Integer))]
        public int StudCreateGreen { get; set; } = 1;

        [StructuresDialog("StudReverse", typeof(TD.Integer))]
        public int StudReverse { get; set; } = 0;


    }
}
