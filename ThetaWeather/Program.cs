using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;


namespace ThetaWeather
{
    class Program
    {
        static void Main(string[] args)
        {
            StreamReader oReader = new StreamReader("..\\..\\Files\\weather.txt", Encoding.UTF8);
            string sInputLine = "";
            string sPreContent = "";

            bool bAppendLine = false;
            DataTable dtWeather = new DataTable();
            int lTableColumnsTxtIndex = -1;
            int lLoopIndex = 0;
            //RL: Loop through each line of the txt file
            while ((sInputLine = oReader.ReadLine()) != null)
            {
                if (bAppendLine && !sInputLine.Trim().StartsWith("</") && sInputLine.Trim() != "")
                {
                   
                    sPreContent += sInputLine + "\n";

                    //RL: We are reading the table headers
                    if (sInputLine.Trim().StartsWith("Dy")) //RL: There is an assumption here that the first table column is 'Dy'. If the feed from API changes, this will need to change. We can set this as a config so we don't change the code each time this changes.
                    {
                        lTableColumnsTxtIndex = lLoopIndex;
                        var cols = sInputLine.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                        for (int col = 0; col < cols.Length; col++)
                            dtWeather.Columns.Add(new DataColumn(cols[col]));

                    } else if (lTableColumnsTxtIndex > -1 && lLoopIndex > lTableColumnsTxtIndex) //RL: this condition satisfies for all actual data table rows in the text file
                    {
                        var colValues = sInputLine.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                        bool bAddline = true; //RL: Flag 
                        Double dblNumber = 0;
                        DataRow dr = dtWeather.NewRow();
                        for (int i = 0; i < colValues.Length; i++)
                        {
                            //RL: We assume that the first column is of type double. If it is not, it must be a dirty data and we will have to exclude it.
                            if (i == 0 && !Double.TryParse(colValues[i], out dblNumber)) 
                            {
                                bAddline = false;
                            }
                            
                            if (i < 3) //RL: We were given information that the first 3 columns are what we needed and that they are of number type
                            {
                                double dblColVal = 0;
                                if (Double.TryParse(colValues[i].Replace("*",""), out dblColVal)) //RL: At this point we are making sure we insert a value of number type
                                {
                                    dr[i] = dblColVal;
                                } else //RL: If it is not of number type then it must be dirty data - we exclude it
                                {
                                    bAddline = false;
                                }
                            } else
                            {
                                dr[i] = colValues[i];
                            }
                            
                        }
                        
                        //RL: If row is not dirty data then we append it.
                        if (bAddline) dtWeather.Rows.Add(dr);
                    }
                }

                if (sInputLine.Trim().StartsWith("<"))
                {

                    if (bAppendLine)    //RL: Condition is true when we get to the closing tag
                    {
                        bAppendLine = false;
                    }
                    else //RL: Condition is true when we get to the opening tag
                    {
                        bAppendLine = true;
                    }

                }
                lLoopIndex++;
            }

            //RL: At this point our datatable is ready
            var sDayWithLeastTempDifference = dtWeather.Select()
                .OrderBy(row => Double.Parse(row.Field<string>("MxT")) - Double.Parse(row.Field<string>("MnT"))) //RL: Order by the difference of Max and Min Temp
                .FirstOrDefault()["Dy"].ToString(); //RL: The day with the least difference will be at the top most row

            //RL: Write the result we want to see
            Console.WriteLine("Day with the least temperature difference is: day " + sDayWithLeastTempDifference);
            Console.ReadLine();
        }
    }
}
