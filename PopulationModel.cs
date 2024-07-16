using System.Collections.Specialized;
using System.Drawing;
using System.Xml.Schema;
using System.IO;

namespace SummerProject_CAST
{
    public class PopulationModel
    {
        Greenflies Juveniles;
        Greenflies Adults;
        Greenflies Seniles;
        public string Data;
        string csvData = "Generation,Juveniles,Adults,Seniles,Total\n";
        int Generations;
        /// <summary>
        /// Create a new model of the greenfly population with given initial values
        /// </summary>
        /// <param name="j">Juveniles</param>
        /// <param name="a">Adults</param>
        /// <param name="s">Seniles</param>
        /// <param name="generations">Number of generations to iterate over. Must be between 5 and 25</param>
        public PopulationModel(Greenflies j, Greenflies a, Greenflies s, int generations = 5)
        {
            Juveniles = j;
            Adults = a;
            Seniles = s;
            Generations = generations;
            if (Generations > 25 || Generations < 5)
            {
                throw new System.Exception("Generation quantity given is either too low (less than 5) or exceeds 25.");
            }
        }
        public void Simulate()
        {
            for (int i=0; i < Generations; i++)
            {
                float total = Juveniles.Population + Adults.Population + Seniles.Population;
                RecordReadable(i);
                RecordCSV(i, total);
                Juveniles.Population = (int)(Adults.Population * Adults.BirthRate);

                Juveniles.Population *= Juveniles.SurvivalRate;
                Adults.Population *= Adults.SurvivalRate;
                Seniles.Population *= Seniles.SurvivalRate;

                Seniles.Population = Adults.Population;
                Adults.Population = Juveniles.Population;
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
