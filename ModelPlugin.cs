using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSM = Tekla.Structures.Model;
using Tekla.Structures.Plugins;
using TSMUI = Tekla.Structures.Model.UI;
using TSG = Tekla.Structures.Geometry3d;
using System.Collections;
using System.Windows;

namespace StudArrayPlugin
{
    public class PluginData
    {
        [StructuresField("Profile")]
        public string Profile;

        [StructuresField("Material")]
        public string Material;

        [StructuresField("StudClass")]
        public string StudClass;

        [StructuresField("StudPrefix")]
        public string StudPrefix;

        [StructuresField("StudHeight")]
        public double StudHeight;

        [StructuresField("StudCrossStep")]
        public double StudCrossStep;

        [StructuresField("StudAlongStep")]
        public double StudAlongStep;

        [StructuresField("StudCrossNum")]
        public int StudCrossNum;

        [StructuresField("StudAlongNum")]
        public int StudAlongNum;

        [StructuresField("StudOffset")]
        public double StudOffset;

        [StructuresField("StudLastNum")]
        public int StudLastNum;

        [StructuresField("StudLastStep")]
        public double StudLastStep;

        [StructuresField("StudCreateBlue")]
        public int StudCreateBlue;

        [StructuresField("StudCreateGreen")]
        public int StudCreateGreen;

        [StructuresField("StudReverse")]
        public int StudReverse;


    }

    [Plugin("StudArrayPlugin")]
    [PluginUserInterface("StudArrayPlugin.MainWindow")]
    [PluginCoordinateSystem(CoordinateSystemType.FROM_FIRST_AND_SECOND_POINT)]
    public class ModelPlugin : PluginBase
    {
        TSM.Model Model { get; set; }
        PluginData Data { get; set; }
        public ModelPlugin(PluginData data)
        {
            Model = new TSM.Model();
            Data = data;
        }
        public override List<InputDefinition> DefineInput()
        {
            List<InputDefinition> inputObjects = new List<InputDefinition>();
            TSMUI.Picker picker = new TSMUI.Picker();

            var mainPart = picker.PickObject(TSMUI.Picker.PickObjectEnum.PICK_ONE_PART, "Укажите деталь для вставки упоров");
            ArrayList pickedPoints = picker.PickPoints(TSMUI.Picker.PickPointEnum.PICK_TWO_POINTS, "Укажите точку начала массива вдоль детали");

            inputObjects.Add(new InputDefinition(mainPart.Identifier));
            inputObjects.Add(new InputDefinition(pickedPoints));

            return inputObjects;
        }

        public override bool Run(List<InputDefinition> inputObjects)
        {
            try
            {
                //Получаем на вход главную деталь
                var identifierPart = (Tekla.Structures.Identifier)inputObjects[0].GetInput();
                var mainPart = Model.SelectModelObject(identifierPart) as TSM.Part;

                //Получаем на вход 2 точки
                ArrayList arrayPoints = inputObjects[1].GetInput() as ArrayList;

                //Создаем упоры
                CreateStudArray(mainPart, arrayPoints);
                Model.CommitChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return true;
        }

        private void CreateStudArray(TSM.Part part,ArrayList pointsArray)
        {
            //Получаем входные данные для метода
            TSG.Point startPoint = pointsArray[0] as TSG.Point;
            TSG.Point endPoint = pointsArray[1] as TSG.Point;

            //Начальная система координат
            TSM.TransformationPlane origPlane = Model.GetWorkPlaneHandler().GetCurrentTransformationPlane();

            //Система координат вставки компонента
            TSG.Vector vectorXInput = new TSG.Vector(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y, endPoint.Z - startPoint.Z);
            vectorXInput.Normalize();
            TSG.Vector vectorYInput = new TSG.Vector(vectorXInput.Y, vectorXInput.X, vectorXInput.Z);
            vectorYInput.Normalize();
            TSG.CoordinateSystem csInput = new TSG.CoordinateSystem(startPoint, vectorXInput, vectorYInput);
            TSM.TransformationPlane tpInput = new TSM.TransformationPlane(csInput);

            Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(tpInput);

            //Система координат детали
            ///Получаем осевые точки детали
            ArrayList partPoints = part.GetCenterLine(true);

            TSG.Point startPartPoint = partPoints[0] as TSG.Point;
            TSG.Point endPartPoint = partPoints[1] as TSG.Point;

            //InsertPoint(startPartPoint);
            //InsertPoint(endPartPoint);

            TSG.Vector vectorXPart = new TSG.Vector(endPartPoint.X - startPartPoint.X, endPartPoint.Y - startPartPoint.Y, endPartPoint.Z - startPartPoint.Z);
            TSG.Vector vectorYPart = new TSG.Vector(vectorXPart.Y, vectorXPart.X, vectorXPart.Z);
            TSG.CoordinateSystem csPart = new TSG.CoordinateSystem(startPartPoint, vectorXPart, vectorYPart);
            TSM.TransformationPlane tpPart = new TSM.TransformationPlane(csPart);
            TSG.Matrix matrixPart = tpPart.TransformationMatrixToGlobal;

            //Находим точку, относительно которой будет строить массив упоров
            TSG.Point originPoint = new TSG.Point(0, 0, 0);
            TSG.Point startArrayPoint = new TSG.Point(originPoint.X, startPartPoint.Y, originPoint.Z);

            ///Построение линий-осей
            TSG.Point axisX = new TSG.Point(startArrayPoint.X + 100, startArrayPoint.Y, startArrayPoint.Z);
            TSG.Point axisY = new TSG.Point(startArrayPoint.X, startArrayPoint.Y + 50, startArrayPoint.Z);

            //InsertPoint(startArrayPoint);
            //InsertLine(startArrayPoint, axisX);
            //InsertLine(startArrayPoint, axisY);
            //MessageBox.Show(startPartPoint.ToString());

            ////Создаем массив упоров
            double pointY1 = 0;
            double pointY2 = 0;
            int iCount = 0;
            int reverse = 1;
            if (Data.StudReverse == 1)
            {
                reverse = -1;
            }

            for (iCount = 0; iCount < Data.StudAlongNum; iCount++)
            {

                if (Data.StudCrossNum == 2)
                {
                    pointY1 = -Data.StudCrossStep / Data.StudCrossNum;
                    pointY2 = -pointY1;
                }

                //Создание упоров
                TSG.Point point1Start = new TSG.Point(startArrayPoint.X + Data.StudOffset + iCount * Data.StudAlongStep, startArrayPoint.Y + pointY1, reverse * startArrayPoint.Z);
                TSG.Point point1End = new TSG.Point(startArrayPoint.X + Data.StudOffset + iCount * Data.StudAlongStep, startArrayPoint.Y + pointY1, reverse * (startArrayPoint.Z + Data.StudHeight));

                TSG.Point point2Start = new TSG.Point(startArrayPoint.X + Data.StudOffset + iCount * Data.StudAlongStep, startArrayPoint.Y + pointY2, reverse * startArrayPoint.Z);
                TSG.Point point2End = new TSG.Point(startArrayPoint.X + Data.StudOffset + iCount * Data.StudAlongStep, startArrayPoint.Y + pointY2, reverse * (startArrayPoint.Z + Data.StudHeight));

                TSM.Beam stud1 = new TSM.Beam(point1Start, point1End);
                TSM.Beam stud2 = new TSM.Beam(point2Start, point2End);

                SetStudParam(stud1);
                SetStudParam(stud2);

                //Вставка упоров со сваркой
                InsertAndWeld(part, stud1);

                if (Data.StudCrossNum == 2)
                {
                    InsertAndWeld(part, stud2);
                }
            }

            //MessageBox.Show(Data.CreateBlue.ToString() + " " + Data.CreateGreen.ToString());

            //Создание концевых упоров
            if (Data.StudLastNum > 0)
            {
                for (int j = 0; j < Data.StudLastNum; j++)
                {

                    TSG.Point point1Start = new TSG.Point(startArrayPoint.X + Data.StudOffset + (iCount - 1) * Data.StudAlongStep + (j + 1) * Data.StudLastStep,
                        startArrayPoint.Y + pointY1, reverse * startArrayPoint.Z);
                    TSG.Point point1End = new TSG.Point(startArrayPoint.X + Data.StudOffset + (iCount - 1) * Data.StudAlongStep + (j + 1) * Data.StudLastStep,
                        startArrayPoint.Y + pointY1,  reverse * (startArrayPoint.Z + Data.StudHeight));

                    TSG.Point point2Start = new TSG.Point(startArrayPoint.X + Data.StudOffset + (iCount - 1) * Data.StudAlongStep + (j + 1) * Data.StudLastStep,
                        startArrayPoint.Y + pointY2,  reverse * startArrayPoint.Z);
                    TSG.Point point2End = new TSG.Point(startArrayPoint.X + Data.StudOffset + (iCount - 1) * Data.StudAlongStep + (j + 1) * Data.StudLastStep,
                        startArrayPoint.Y + pointY2,  reverse * (startArrayPoint.Z + Data.StudHeight));

                    TSM.Beam stud1 = new TSM.Beam(point1Start, point1End);
                    TSM.Beam stud2 = new TSM.Beam(point2Start, point2End);

                    SetStudParam(stud1);
                    SetStudParam(stud2);

                    InsertAndWeld(part, stud1);

                    if (Data.StudCrossNum == 2)
                    {
                        InsertAndWeld(part, stud2);
                    }
                    //Вставка упоров со сваркой
                    if (j == Data.StudLastNum - 1)
                    {
                        stud1.Class = "4";
                        stud2.Class = "3";

                        stud1.Modify();
                        stud1.Delete();

                        if (Data.StudCrossNum == 2)
                        {
                            stud2.Modify();
                            stud2.Delete();
                        }

                        if (Data.StudCreateBlue == 1)
                        {
                            InsertAndWeld(part, stud1);
                        }

                        if (Data.StudCreateGreen == 1)
                        {
                            InsertAndWeld(part, stud2);
                        }
                    }

                }
            }

            Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(origPlane);

        }

        private void InsertPoint(TSG.Point point)
        {
            TSM.ControlPoint controlPoint = new TSM.ControlPoint(point);
            controlPoint.Insert();
        }

        private void InsertLine(TSG.Point point1, TSG.Point point2)
        {
            TSG.LineSegment lineSegment = new TSG.LineSegment(point1, point2);
            TSM.ControlLine line = new TSM.ControlLine(lineSegment, false);
            line.LineType = TSM.ControlObjectLineType.SolidLine;
            line.Color = TSM.ControlLine.ControlLineColorEnum.RED;
            line.Insert();
        }

        private void SetStudParam (TSM.Beam stud)
        {
            stud.Name = "Упор";
            stud.Profile.ProfileString = Data.Profile;
            stud.Material.MaterialString = Data.Material;
            stud.Class = Data.StudClass;
            stud.PartNumber.StartNumber = 1;
            stud.AssemblyNumber.StartNumber = 1;
            stud.PartNumber.Prefix = Data.StudPrefix;
            stud.AssemblyNumber.Prefix = "";
            stud.Position.Plane = TSM.Position.PlaneEnum.MIDDLE;
            stud.Position.Depth = TSM.Position.DepthEnum.MIDDLE;
        }

        private void InsertAndWeld (TSM.Part mainPart, TSM.Beam stud)
        {
            stud.Insert();

            TSM.Weld weld = new TSM.Weld();
            weld.ShopWeld = true;
            weld.MainObject = mainPart;
            weld.SecondaryObject = stud;
            weld.Insert();
        }

    }
}
