using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blynclight;

namespace BetterBlync
{
    public class BlyncControl
    {
        private BlynclightController blync;
        private int lightCount, deviceSelection;

        public enum BlyncColor
        {
            Green,
            Blue,
            Purple,
            Yellow,
            Red
        }

        public BlyncControl()
        {
            blync = new BlynclightController();
            lightCount = findBlyncLights();
            if(lightCount == 0 )
            {
                // TODO create custom exception
            }
            // TODO see what happens with multiple connected lights
            // if ( lightCount == 1 ) deviceSelection = 0;
            deviceSelection = 0;

            // Default color is green
            ChangeToGreen();
            CurrentColor = BlyncColor.Green;
        }

        public BlyncColor CurrentColor { get; private set; }

        private int findBlyncLights()
        {
            return blync.InitBlyncDevices();
        }

        public void CloseDevices()
        {
            blync.CloseDevices( lightCount );
        }

        public void ChangeToGreen()
        {
            blync.TurnOnGreenLight( deviceSelection );
            CurrentColor = BlyncColor.Green;
        }

        public void ChangeToBlue()
        {
            blync.TurnOnBlueLight( deviceSelection );
            CurrentColor = BlyncColor.Blue;
        }

        public void ChangeToYellow()
        {
            blync.TurnOnYellowLight( deviceSelection );
            CurrentColor = BlyncColor.Yellow;
        }

        public void ChangeToPurple()
        {
            blync.TurnOnMagentaLight( deviceSelection );
            CurrentColor = BlyncColor.Purple;
        }

        public void ChangeToRed()
        {
            blync.TurnOnRedLight( deviceSelection );
            CurrentColor = BlyncColor.Red;
        }

        public void ResetLight()
        {
            blync.ResetLight( deviceSelection );
            ChangeToGreen();
            CurrentColor = BlyncColor.Green;
        }

        public void TurnOffLight()
        {
            blync.ResetLight( deviceSelection );
        }
    }
}
