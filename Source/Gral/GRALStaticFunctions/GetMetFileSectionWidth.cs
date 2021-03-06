#region Copyright
///<remarks>
/// <GRAL Graphical User Interface GUI>
/// Copyright (C) [2019]  [Dietmar Oettl, Markus Kuntner]
/// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by
/// the Free Software Foundation version 3 of the License
/// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
/// You should have received a copy of the GNU General Public License along with this program.  If not, see <https://www.gnu.org/licenses/>.
///</remarks>
#endregion

/*
 * Created by SharpDevelop.
 * User: Markus
 * Date: 26.01.2019
 * Time: 17:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;

namespace GralStaticFunctions
{
	/// <summary>
	/// Analyze if there is a binned Meteo Time Series
	/// </summary>
	public class GetMetFileSectionWidth
	{
		/// <summary>
		/// Returns the section width of a MettimeSeries
		/// </summary>
		///<returns>Yields a new Point or break</returns>  
		public static float GetMetSectionWidth(List<GralData.WindData> MeteoTimeSeries)
		{
            float sectionWidth = 10;

            if (MeteoTimeSeries.Count < 4)
            {
                return 1;
            }

            // Analyze the minimum and maximum width of all entries in MeteoTimeSeries
            HashSet<int> directionValues = new HashSet<int>();
            foreach (GralData.WindData data in MeteoTimeSeries)
            {
                directionValues.Add((int) (data.Dir));
            }

            if (directionValues.Count < 3)
            {
                return 1;
            }

            // create list for sorting
            List<int> sorted = new List<int>();
            foreach(int _val in directionValues)
            {
                sorted.Add(_val);
            }
            sorted.Sort();

            // calculate min and max value of directions
            int min = 1000000;
            int mean = 0;
            for(int i = 0; i < sorted.Count - 1; i++)
            {
                int delta = sorted[i + 1] - sorted[i];
                min = Math.Min(min, delta);
                mean += delta;
            }        
            mean /= (sorted.Count - 1);

            sorted.Clear();
            sorted.TrimExcess();
            directionValues.Clear();
            directionValues.TrimExcess();
            //System.Windows.MessageBox.Show(min.ToString() + "/" + mean.ToString());

            if (Math.Abs(min - mean) > 0.1)
            {
                sectionWidth = 1;  // not classified??
            }
            else
            {
                sectionWidth = min;
            }
            return sectionWidth;
		}

	}
}
