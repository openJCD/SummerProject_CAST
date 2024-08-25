using SharpDX;
using System;
using System.IO;
using System.Text;

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
        int _diseasePoint;
        string csvData = "Generation,Juveniles,Adults,Seniles,Total\n";
        int Generations;

        /// <summary>
        /// Create a new model of the greenfly population with given initial values
        /// </summary>
        /// <param name="j">Juveniles</param>
        /// <param name="a">Adults</param>
        /// <param name="s">Seniles</param>
        /// <param name="generations">Number of generations to iterate over. Must be between 5 and 25</param>
        /// <param name="diseaseMod">Disease factor - affects population every/param>
        public PopulationModel(Greenflies j, Greenflies a, Greenflies s, int diseasePoint, int generations = 5)
        {
            Juveniles = j;
            Adults = a;
            Seniles = s;
            Generations = generations;
            _diseasePoint = diseasePoint;
            if (Generations > 25 || Generations < 5)
            {
                throw new System.Exception("Generation quantity given is either too low (less than 5) or exceeds 25.");
            }
            // generate random disease factor
            _diseaseFactor = new Random().NextFloat(0.2f, 0.5f);
        }

        /// <summary>
        /// Run simulation (important bit)
        /// </summary>
        public void Simulate()
        {
            // reset data strings
            Data = "";
            csvData = CreateHeaderInfo();
            
            // record the first line of the simulation with the initial values
            float total = Juveniles.Population + Adults.Population + Seniles.Population;
            RecordCSV(0, total);
            RecordReadable(0);
            bool hitDisease = false;
            // loop through generations
            for (int i=1; i <= Generations; i++)
            {
                // record the generation as the 'last one'
                // for use in the next generation's pop calculations 
                // (this was implemented because previous results with default values
                // did not match those in the table on the PDF spec)
                Greenflies prev_a = Adults;
                Greenflies prev_s = Seniles;
                Greenflies prev_j = Juveniles;
                
                // calculations !!
                Adults.Population = prev_j.Population * Adults.SurvivalRate;

                // only apply disease factor if threshhold is hit for seniles using ternary
                Seniles.Population = hitDisease ?
                    prev_a.Population * Seniles.SurvivalRate * _diseaseFactor :
                    prev_a.Population * Seniles.SurvivalRate;

                // only apply disease factor if threshhold is hit for juveniles using ternary
                Juveniles.Population = hitDisease?
                    (float)(prev_a.Population * Adults.BirthRate * _diseaseFactor) :
                    (float)(prev_a.Population * Adults.BirthRate);

                if (total >= _diseasePoint)
                    hitDisease = true;

                total = Juveniles.Population + Adults.Population + Seniles.Population;

                
                // record current generation
                RecordCSV(i, total);
                RecordReadable(i);
            }
        }

        /// <summary>
        /// Method that creates a string containing all variables used in the simulation
        /// (Such as initial populations, survival/birth rates, disease factor/threshhold)
        /// </summary>
        /// <returns>The string header data</returns>
        string CreateHeaderInfo()
        {
            StringBuilder d = new StringBuilder()
                .Append("# Initial Variable Data: ,").AppendLine()
                .Append("Greenflies, Population, Survival Rate, Birth Rate, Disease Factor, Disease Threshhold").AppendLine()
                .Append("Juveniles Initial,").Append(Juveniles.Population + ",").Append(Juveniles.SurvivalRate).AppendLine()
                .Append("Adults Initial,").Append(Adults.Population + ",").Append(Adults.SurvivalRate + ",").Append(Adults.BirthRate).AppendLine()
                .Append("Seniles Initial,").Append(Seniles.Population + ",").Append(Seniles.SurvivalRate).AppendLine()
                .Append(",,,,").Append(_diseaseFactor).Append("," + _diseasePoint).AppendLine()
                .AppendLine("Generation,Juveniles,Adults,Seniles,Total");
            return d.ToString();
        }
        /// <summary>
        /// places a file with the given name in the app directory 'export/' folder
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="allow_overwrite"></param>
        /// <returns>ExportResult enum - allows for the creation of a 'confirm overwrite' dialog window</returns>
        public ExportResult ExportCSV(string filename, bool allow_overwrite = false)
        {
            
            if (!Directory.Exists("export"))
            {
                Directory.CreateDirectory("export");
                filename = "export/" + filename;
            } else
            {
                filename = "export/" + filename;
            }
            try
            {
                if (File.Exists(filename))
                {
                    if (!allow_overwrite)
                        return ExportResult.FileExists;
                    Stream fs = new FileStream(filename, FileMode.Open);
                    StreamWriter sw = new StreamWriter(fs);
                
                    sw.Write(csvData);
                    sw.FlushAsync();
                    sw.Close();
                    sw.Dispose();
                }
                else
                {
                    Stream fs = new FileStream(filename, FileMode.Create);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.Write(csvData);
                    sw.FlushAsync();
                    sw.Close();
                    sw.Dispose();
                }
            } catch
            {
                return ExportResult.Failure;
            }
            return ExportResult.Success;
        }

        /// <summary>
        /// Records simulation data for this gen 
        /// to a string in readable format 
        /// ('|' char separator)
        /// </summary>
        /// <param name="generation">current generation being recorded</param>
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
    /// <summary>
    /// struct that contains data for a greenfly population
    /// </summary>
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
