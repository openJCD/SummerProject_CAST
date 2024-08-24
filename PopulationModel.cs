using System.Collections.Specialized;
using System.Drawing;
using System.Xml.Schema;
using System.IO;
using Windows.System.Update;

namespace SummerProject_CAST
{
    /// <summary>
    /// Primary part of the application
    /// </summary>
    public class PopulationModel
    {
        Greenflies Juveniles;
        Greenflies Adults;
        Greenflies Seniles;
        public string Data;
        float _diseaseFactor;

        string csvData = "Generation,Juveniles,Adults,Seniles,Total\n";
        int Generations;
        /// <summary>
        /// Create a new model of the greenfly population with given initial values
        /// </summary>
        /// <param name="j">Juveniles</param>
        /// <param name="a">Adults</param>
        /// <param name="s">Seniles</param>
        /// <param name="generations">Number of generations to iterate over. Must be between 5 and 25</param>
        public PopulationModel(Greenflies j, Greenflies a, Greenflies s, int generations = 5, float diseaseMod = 0.5f)
        {
            Juveniles = j;
            Adults = a;
            Seniles = s;
            Generations = generations;
            _diseaseFactor = diseaseMod;
            if (Generations > 25 || Generations < 5)
            {
                throw new System.Exception("Generation quantity given is either too low (less than 5) or exceeds 25.");
            }
            if (diseaseMod > 0.5f || diseaseMod <0.2f)
            {
                throw new System.Exception("Disease factor either exceeds 0.5 or is lower than 0.2");
            }
        }
        public void Simulate()
        {
            Data = "";
            csvData = "Generation,Juveniles,Adults,Seniles,Total\n";
            float total = Juveniles.Population + Adults.Population + Seniles.Population;
            RecordCSV(0, total);
            RecordReadable(0);
            for (int i=1; i <= Generations; i++)
            {
                Greenflies prev_a = Adults;
                Greenflies prev_s = Seniles;
                Greenflies prev_j = Juveniles;
                
                Adults.Population *= Adults.SurvivalRate;
                Seniles.Population *= Seniles.SurvivalRate * _diseaseFactor;
                Juveniles.Population *= Juveniles.SurvivalRate * _diseaseFactor;
                
                Adults.Population = prev_j.Population;
                Seniles.Population = prev_a.Population;
                Juveniles.Population = (int)(prev_a.Population * Adults.BirthRate);
                total = Juveniles.Population + Adults.Population + Seniles.Population;
                RecordCSV(i, total);
                RecordReadable(i);
            }
        }
        public ExportResult ExportCSV(string filepath, bool allow_overwrite = false)
        {
            try
            {
                if (File.Exists(filepath))
                {
                    if (!allow_overwrite)
                        return ExportResult.FileExists;
                    Stream fs = new FileStream(filepath, FileMode.Open);
                    StreamWriter sw = new StreamWriter(fs);
                
                    sw.Write(csvData);
                    sw.FlushAsync();
                }
                else
                {
                    Stream fs = new FileStream(filepath, FileMode.Create);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.Write(csvData);
                    sw.FlushAsync();
                }
            } catch
            {
                return ExportResult.Failure;
            }
            return ExportResult.Success;
        }
        private void RecordReadable (int generation)
        {
            Data += "Gen " + generation.ToString() + "| Juveniles: "
                + Juveniles.Population.ToString() + "| Adults: " +
                Adults.Population.ToString() + "| Seniles: " +
                Seniles.Population.ToString() +"\r\n";
        }
        private void RecordCSV (int generation, float total)
        {
            float[] values = { generation, Juveniles.Population, Adults.Population, Seniles.Population, total };
            csvData += string.Join(",", values) + "\n";
        }
    }
    public struct Greenflies
    {
        int InitialPopulation;
        public float Population { get; set; }

        public float SurvivalRate { get; private set; }

        public float? BirthRate { get; private set; }
        public Greenflies (int initial, float survival, float? birth)
        {
            InitialPopulation = initial;
            Population = InitialPopulation;
            SurvivalRate = survival;
            BirthRate = birth;
        }
    }

}
