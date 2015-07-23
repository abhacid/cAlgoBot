#region licence
//The MIT License (MIT)
//Copyright (c) 2014 abdallah HACID, https://www.facebook.com/ab.hacid

//Permission is hereby granted, free of charge, to any person obtaining a copy of this software
//and associated documentation files (the "Software"), to deal in the Software without restriction,
//including without limitation the rights to use, copy, modify, merge, publish, distribute,
//sublicense, and/or sell copies of the Software, and to permit persons to whom the Software
//is furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all copies or
//substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
//BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
//DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

// Project Hosting for Open Source Software on Github : https://github.com/abhacid/cAlgoBot 
#endregion

#region description
/// Based on ADXR by qualitiedx2 (http://ctdn.com/algos/indicators/show/19)
/// Average Directional Movement Index Rating (ADXR) is a smoothed version of ADX indicator and is used as a rating
/// of the Directional Movement while smoothing out ADX values.
///
/// ADX, ADXR, DMI : Directional Movement : Application au trading et à l’investissement :
/// Le DMI peut être utile dans plusieurs cas :
/// Pour déterminer une tendance et mettre en évidence la force de celle ci : Comparaison de l’ADX et l’ADXR. Si ces deux indicateurs bornés (de 0 à 100)
/// se situent en dessous de 20, le marché est considéré sans tendance. 
/// 
/// Si l’ADX et l’ADXR sont au dessus de 20, le marché est en tendance.
/// 
/// L’ADXR correspond à une moyenne de l’ADX. Le croisement à la baisse de l’ADX par rapport à l’ADXR est un signal de faiblesse de la tendance.
/// L’ADXR et l’ADX ne donnent pas le sens de la tendance mais la force de celle ci. Une évolution de l’ADX au dessus de 40 indique une forte tendance 
/// qui peut être haussière ou baissière.
/// 
/// Ces deux indicateurs aident le trader dans les choix d’outils et de méthodes qui peuvent être différents si le marché évolue ou non en tendance.
/// Pour déterminer le rapport de force haussier/baissier : comparaison du DI– et du DI+. Le DI+ mesure la force des mouvements haussiers, le DI– 
/// la force des mouvements baissiers. Le calcul de ces deux indicateurs se fait sur une période déterminée (14 par défaut et selon Welles Wilder). 
/// Le signal d’achat ou de vente est déterminé par le croisement du DI+ et du DI–.
/// Le signal d’achat est généré quand le DI+ passe au dessus du DI–
/// Le signal de vente est généré quand le DI+ tombe en dessous du DI–.
/// Ce signal sera plus fort si l’ADX est à un niveau élevé.
#endregion

using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, AccessRights = AccessRights.None)]
    [Levels(20, 40)]
    /// <summary>
    /// Directional Movement System Rating
    /// </summary>
	public class DirectionalMovementSystemRating : Indicator
    {
        [Parameter("Period", DefaultValue = 14)]
        public int Period { get; set; }

        [Output("ADX Rating", Color = Colors.Blue, Thickness = 2)]
        public IndicatorDataSeries ADXR { get; set; }

		[Output("ADX", Color = Colors.CornflowerBlue, Thickness=1)]
		public IndicatorDataSeries ADX { get; set; }

        [Output("Di-", Color = Colors.Red, PlotType=PlotType.DiscontinuousLine)]
        public IndicatorDataSeries DIMinus { get; set; }

        [Output("Di+", Color = Colors.Green, PlotType=PlotType.DiscontinuousLine)]
        public IndicatorDataSeries DIPlus { get; set; }

        [Output("Diff", Color = Colors.DarkBlue, IsHistogram = true,Thickness=2)]
        public IndicatorDataSeries Diff { get; set; }
        

        private DirectionalMovementSystem _dms;

        protected override void Initialize()
        {
            _dms = Indicators.DirectionalMovementSystem(14);
        }

        public override void Calculate(int index)
        {
            ADX[index] = _dms.ADX[index];
            ADXR[index] = (_dms.ADX[index] + _dms.ADX[index - Period]) / 2;

            DIMinus[index] = _dms.DIMinus[index];
            DIPlus[index] = _dms.DIPlus[index];

            Diff[index] = _dms.DIPlus[index] - _dms.DIMinus[index];
        }
    }
}
