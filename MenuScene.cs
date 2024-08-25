using HyperLinkUI.Engine.Animations;
using HyperLinkUI.Engine.GUI;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoTween;
using System.Linq;

namespace SummerProject_CAST
{
    public class MenuScene
    {

        UIRoot UI;

        TextLabel label_generations;
        TextInput input_generations;

        TextInput input_disease;

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

        WindowContainer dialog_export_overwrite;
        WindowContainer dialog_export;

        Greenflies juveniles_default;
        Greenflies adults_default;
        Greenflies seniles_default;
        int generations_default;
        int disease_default = 100;
        public MenuScene()
        {
            UIEventHandler.OnButtonClick += MenuScene_OnButtonClick;
            UIEventHandler.OnTextFieldSubmit += MenuScene_OnTextFieldSubmit;
            juveniles_default = new Greenflies(10, 1, null);
            adults_default = new Greenflies(10, 1, 2f);
            seniles_default = new Greenflies(10, 0, 1);
            generations_default = 5;
        }

        public void Load(UIRoot ui, ContentManager content)
        {
            UI = ui;
            Texture2D ns = content.Load<Texture2D>("Textures/NS_TEXTINPUT");
            Container c = new Container(ui, 0, 0, 300, 275, AnchorType.CENTRE);
            c.Open();
            c.DrawBorder = true;
            c.RenderBackgroundColor = true;
            c.ClipContents = true;
            c.EnableNineSlice(ns);
            
            Button btn_open_sim_values = new Button(c, "Set Simulation Values", 0, -90, 250, 50, AnchorType.CENTRE, EventType.OpenWindow, "sim_value_dialog");

            Button btn_start_sim = new Button(c, "Start Simulation", 0, -30, 200, 50, AnchorType.CENTRE, EventType.OpenWindow, "sim_data_dialog");

            Button btn_export_csv = new Button(c, "Export CSV...", 0, 30, 200, 50, AnchorType.CENTRE, EventType.OpenWindow, "export_csv");

            Button btn_quit = new Button(c, "Quit", 0, 90, 200, 50, AnchorType.CENTRE, EventType.QuitGame, "quit");

            #region dialog for input of simulation initial values
            WindowContainer sim_value_dialog = new WindowContainer(ui, -100, 50, 500, 200, "sim_value_dialog", "Enter Initial Simulation Values", AnchorType.TOPRIGHT);
            label_generations = new TextLabel(sim_value_dialog, "Generations:", 20, 25);
            input_generations = new TextInput(sim_value_dialog, 20, 50, 80, AnchorType.TOPLEFT).SetCharLimit(2);

            new TextLabel(sim_value_dialog, "Disease Threshhold: ", relativex: 20, relativey: -65, anchorType: AnchorType.BOTTOMLEFT) ;
            input_disease = new TextInput(sim_value_dialog, 20, -25, 80).SetCharLimit(10);

            label_juveniles = new TextLabel(sim_value_dialog, "Juveniles:", 150, 25);
            input_juveniles_pop = new TextInput(sim_value_dialog, 150, 50, 80, AnchorType.TOPLEFT, "Population").SetCharLimit(10);
            input_juveniles_survival = new TextInput(sim_value_dialog, 150, 85, 80, AnchorType.TOPLEFT, "Survival R.").SetCharLimit(10);

            label_adults = new TextLabel(sim_value_dialog, "Adults:", 270, 25);
            input_adults_pop = new TextInput(sim_value_dialog, 270, 50, 80, AnchorType.TOPLEFT, "Population").SetCharLimit(10);
            input_adults_survival = new TextInput(sim_value_dialog, 270, 85, 80, AnchorType.TOPLEFT, "Survival R.").SetCharLimit(10);
            input_adults_birthrate = new TextInput(sim_value_dialog, 270, 120, 80, AnchorType.TOPLEFT, "Birth Rate").SetCharLimit(10);

            label_seniles = new TextLabel(sim_value_dialog, "Seniles:", 380, 25);
            input_seniles_pop = new TextInput(sim_value_dialog, 380, 50, 80, AnchorType.TOPLEFT, "Population").SetCharLimit(10);
            input_seniles_survival = new TextInput(sim_value_dialog, 380, 85, 80, AnchorType.TOPLEFT, "Survival R.").SetCharLimit(10);
            Button btn_apply_sim_values = new Button(sim_value_dialog, "Apply", -10, -10, AnchorType.BOTTOMRIGHT, EventType.None, "apply_sim_values");
            sim_value_dialog.Close();
            sim_value_dialog.EnableCloseButton();
            #endregion

            #region dialog for output of simulation values
            WindowContainer sim_data_dialog = new WindowContainer(ui, 0, 0, 450, 350, "sim_data_dialog", "Simulation Results", AnchorType.BOTTOMRIGHT);
            output_data = new TextLabel(sim_data_dialog, "", 5, 30);
            
            sim_data_dialog.Close();
            sim_data_dialog.EnableCloseButton();
            sim_data_dialog.Resizeable = true;
            sim_data_dialog.ClipContents = true;
            #endregion

            #region dialog for file export
            dialog_export = new WindowContainer(ui, 10, -10, 400, 100, "export_csv", "Enter filename for export...", AnchorType.BOTTOMLEFT);
            dialog_export.Close();
            input_csv_export = new TextInput(dialog_export, 0, 0, 300, AnchorType.CENTRE, "filepath", 4);
            input_csv_export.Height = 20;
            #endregion

            Keyframes.StaggeredCustom(
                new Control[] { btn_apply_sim_values, btn_start_sim, btn_export_csv, btn_quit }, 
                -300, 0, 0.5f, Ease.InCubic, 0.1f);
        }

        public void MenuScene_OnButtonClick (object sender, OnButtonClickEventArgs e)
        {
            if (e.tag == "apply_sim_values")
            {
                try
                {
                    juveniles_default = new Greenflies(int.Parse(input_juveniles_pop.InputText), float.Parse(input_juveniles_survival.InputText), null);
                    adults_default = new Greenflies(int.Parse(input_adults_pop.InputText), float.Parse(input_adults_survival.InputText), float.Parse(input_adults_birthrate.InputText));
                    seniles_default = new Greenflies(int.Parse(input_seniles_pop.InputText), float.Parse(input_seniles_survival.InputText), null);
                    generations_default = int.Parse(input_generations.InputText);
                    disease_default = int.Parse(input_disease.InputText);
                }
                catch
                {
                    ErrorDialog("Check simulation input values");
                }
            }
            if (e.tag == "sim_data_dialog" && e.event_type == EventType.OpenWindow)
            {
                ScenePopModel = new PopulationModel(juveniles_default, adults_default, seniles_default, disease_default, generations_default);
                ScenePopModel.Simulate();
                output_data.Text = ScenePopModel.Data;
            }
            if (e.tag == "export_confirm_no" || e.tag == "export_confirm_yes")
            {
                dialog_export_overwrite.Dispose();
                if (e.tag == "export_confirm_yes")
                {
                    ScenePopModel.ExportCSV(input_csv_export.InputText, true);
                    dialog_export.Close();
                }
            }
            if (e.tag == "err")
            {
                UI.ChildContainers.FirstOrDefault(c => c.Tag == "err").Dispose();
            }
        }
        public void MenuScene_OnTextFieldSubmit(object sender, MiscTextEventArgs e)
        {
            if (sender == input_csv_export)
            {
                if (ScenePopModel == null)
                {
                    ErrorDialog("No data yet created for export.");
                    return;
                }

                ExportResult exp = ScenePopModel.ExportCSV(input_csv_export.InputText);
                if (exp == ExportResult.FileExists) 
                {
                    // create a yes/no dialog for this
                    CreateOverwriteDialog();
                    UIEventHandler.sendDebugMessage(this, "File already exists. \n(Implement dialog for check)");
                } else if (exp == ExportResult.Failure)
                {
                    ErrorDialog("Error occurred in file export.");
                    UIEventHandler.sendDebugMessage(this, "An Error occurred. Could not save CSV file.");
                } else if (exp == ExportResult.Success)
                {
                    dialog_export.Close();
                }
            }
        }
        public void CreateOverwriteDialog()
        {
            dialog_export_overwrite = new WindowContainer(UI, 0, 0, 150, 80, "export_confirm", "Confirm export overwrite?");
            Button no = new Button(dialog_export_overwrite, "No", 20, -10, AnchorType.BOTTOMLEFT, EventType.None, "export_confirm_no");
            Button yes = new Button(dialog_export_overwrite, "Yes", -20, -10, AnchorType.BOTTOMRIGHT, EventType.None, "export_confirm_yes");
        }
        public void ErrorDialog(string message)
        {
            var c = new WindowContainer(UI, 0, 0, 200, 80, "err", "Error: ", AnchorType.CENTRE) { Resizeable = true, ClipContents = true, };
            new TextLabel(c, message, 0, 10, AnchorType.CENTRE);
            c.EnableCloseButton(7);
        }
    }
}
