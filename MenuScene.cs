using HyperLinkUI.Engine.GUI;
using Microsoft.Xna.Framework.Graphics;

namespace SummerProject_CAST
{
    public class MenuScene
    {

        UIRoot UI;
        TextLabel label_generations;
        TextInput input_generations;
        
        TextLabel label_juveniles;
        TextInput input_juveniles_pop;
        TextInput input_juveniles_survival;

        TextLabel label_adults;
        TextInput input_adults_pop;
        TextInput input_adults_survival;
        TextInput input_adults_birthrate;
        
        TextLabel label_seniles;
        TextInput input_seniles_pop;
        TextInput input_seniles_survival;

        public PopulationModel ScenePopModel;

        TextLabel output_data;

        TextInput input_csv_export;

        Greenflies juveniles_default;
        Greenflies adults_default;
        Greenflies seniles_default;
        int generations_default;
        public MenuScene()
        {
            UIEventHandler.OnButtonClick += MenuScene_OnButtonClick;
            UIEventHandler.OnTextFieldSubmit += MenuScene_OnTextFieldSubmit;
            juveniles_default = new Greenflies(10, 1, null);
            adults_default = new Greenflies(10, 1, 2f);
            seniles_default = new Greenflies(10, 0, 1);
            generations_default = 5;
        }

        public void Load(UIRoot ui)
        {
            UI = ui;
            Container c = new Container(ui, 0, 0, 300, 300, AnchorType.CENTRE);
            c.DrawBorder = true;
            c.RenderBackgroundColor = true;
            //c.IsOpen = false;
            Texture2D ns = ui.Settings.ContentManager.Load<Texture2D>("Textures/NS_TEXTINPUT");
            Texture2D nw_w = ui.Settings.ContentManager.Load<Texture2D>("Textures/NS_WINDOW");
            c.EnableNineSlice(ns);
            Button btn_open_sim_values = new Button(c, "Set Simulation Values", 0, -90, 250, 50, AnchorType.CENTRE, EventType.OpenWindow, "sim_value_dialog");

            Button btn_start_sim = new Button(c, "Start Simulation", 0, -30, 200, 50, AnchorType.CENTRE, EventType.OpenWindow, "sim_data_dialog");

            Button btn_export_csv = new Button(c, "Export CSV...", 0, 30, 200, 50, AnchorType.CENTRE, EventType.OpenWindow, "export_csv");

            Button btn_quit = new Button(c, "Quit", 0, 90, 200, 50, AnchorType.CENTRE, EventType.QuitGame, "quit");

            #region dialog for input of simulation initial values
            WindowContainer sim_value_dialog = new WindowContainer(ui, -100, 50, 500, 200, "sim_value_dialog", "Enter Initial Simulation Values", AnchorType.TOPRIGHT);
            sim_value_dialog.EnableNineSlice(nw_w); 
            label_generations = new TextLabel(sim_value_dialog, "Generations:", 20, 50);
            input_generations = new TextInput(sim_value_dialog, 20, 75, 80, AnchorType.TOPLEFT);
            input_generations.EnableNineSlice(ns);

            label_juveniles = new TextLabel(sim_value_dialog, "Juveniles:", 150, 50);
            input_juveniles_pop = new TextInput(sim_value_dialog, 150, 75, 80, AnchorType.TOPLEFT);
            input_juveniles_pop.EnableNineSlice(ns);
            input_juveniles_survival = new TextInput(sim_value_dialog, 150, 110, 80, AnchorType.TOPLEFT);
            input_juveniles_survival.EnableNineSlice(ns);

            label_adults = new TextLabel(sim_value_dialog, "Adults:", 270, 50);
            input_adults_pop = new TextInput(sim_value_dialog, 270, 75, 80, AnchorType.TOPLEFT);
            input_adults_pop.EnableNineSlice(ns);
            input_adults_survival = new TextInput(sim_value_dialog, 270, 110, 80, AnchorType.TOPLEFT);
            input_adults_survival.EnableNineSlice(ns);
            input_adults_birthrate = new TextInput(sim_value_dialog, 270, 150, 80, AnchorType.TOPLEFT);
            input_adults_birthrate.EnableNineSlice(ns);

            label_seniles = new TextLabel(sim_value_dialog, "Seniles:", 380, 50);
            input_seniles_pop = new TextInput(sim_value_dialog, 380, 75, 80, AnchorType.TOPLEFT);
            input_seniles_pop.EnableNineSlice(ns);
            input_seniles_survival = new TextInput(sim_value_dialog, 380, 110, 80, AnchorType.TOPLEFT);
            input_seniles_survival.EnableNineSlice(ns);
            Button btn_apply_sim_values = new Button(sim_value_dialog, "Apply", -10, -10, 70, 50, AnchorType.BOTTOMRIGHT, EventType.None, "apply_sim_values");
            sim_value_dialog.Close();
            #endregion

            #region dialog for output of simulation values
            WindowContainer sim_data_dialog = new WindowContainer(ui, 0, 0, 500, 600, "sim_data_dialog", "Simulation Results", AnchorType.BOTTOMRIGHT);
            
            output_data = new TextLabel(sim_data_dialog, "", 5, 30);
            
            sim_data_dialog.Close();
            sim_data_dialog.Resizeable = true;
            sim_data_dialog.ClipContents = true;
            #endregion

            #region dialog for file export
            WindowContainer dialog_export = new WindowContainer(ui, 10, -10, 400, 100, "export_csv", "Enter filename for export...", AnchorType.BOTTOMLEFT);
            dialog_export.Close();
            input_csv_export = new TextInput(dialog_export, 0, 0, 300, AnchorType.CENTRE, "filepath", 4);
            #endregion

        }

        public void MenuScene_OnButtonClick (object sender, OnButtonClickEventArgs e)
        {
            if (e.tag == "apply_sim_values")
            {
                // survival and birth rates are set to placeholder values
                juveniles_default = new Greenflies(int.Parse(input_juveniles_pop.InputText), float.Parse(input_juveniles_survival.InputText), null);
                adults_default = new Greenflies(int.Parse(input_adults_pop.InputText), float.Parse(input_adults_survival.InputText), float.Parse(input_adults_birthrate.InputText));
                seniles_default = new Greenflies(int.Parse(input_seniles_pop.InputText), float.Parse(input_seniles_survival.InputText), null);
                generations_default = int.Parse(input_generations.InputText);
            }
            if (e.tag == "sim_data_dialog" && e.event_type ==EventType.OpenWindow)
            {
                ScenePopModel = new PopulationModel(juveniles_default, adults_default, seniles_default, generations_default);
                ScenePopModel.Simulate();
                output_data.Text = ScenePopModel.Data;
            }
        }
        public void MenuScene_OnTextFieldSubmit(object sender, MiscTextEventArgs e)
        {
            if (sender == input_csv_export)
            {
                ExportResult exp = ScenePopModel.ExportCSV(input_csv_export.InputText);
                if (exp == ExportResult.FileExists) 
                {
                    // create a yes/no dialog for this
                    UIEventHandler.sendDebugMessage(this, "File already exists. \n(Implement dialog for check)");
                } else if (exp == ExportResult.Failure)
                {
                    UIEventHandler.sendDebugMessage(this, "An Error occurred. Could not save CSV file.");
                }
            }
        }
        public void CreateYesNoDialog(string title)
        {

        }
    }
}
