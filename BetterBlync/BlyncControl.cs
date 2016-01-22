using Blynclight;

namespace BetterBlync
{
    public class BlyncControl
    {
        private BlynclightController blync;
        private int lightCount, deviceSelection;

        public BlyncColor CurrentColor { get; private set; }

        public enum BlyncColor
        {
            Green,
            Blue,
            Cyan,
            Purple,
            Yellow,
            Red,
            White
        }

        public BlyncControl()
        {
            blync = new BlynclightController();
            lightCount = findBlyncLights();
            if ( lightCount == 0 )
            {
                // TODO create custom exception
            }
            // TODO see what happens with multiple connected lights
            // if ( lightCount == 1 ) deviceSelection = 0;
            deviceSelection = 0;

            // Default color is green
            changeToGreen();
            CurrentColor = BlyncColor.Green;
        }

        public void SetColor(BlyncColor color)
        {
            switch ( color )
            {
                case BlyncColor.Blue:
                    changeToBlue();
                    break;

                case BlyncColor.Cyan:
                    changeToCyan();
                    break;

                case BlyncColor.Green:
                    changeToGreen();
                    break;

                case BlyncColor.Purple:
                    changeToPurple();
                    break;

                case BlyncColor.Red:
                    changeToRed();
                    break;

                case BlyncColor.White:
                    changeToWhite();
                    break;

                case BlyncColor.Yellow:
                    changeToYellow();
                    break;
            }
        }

        private int findBlyncLights()
        {
            return blync.InitBlyncDevices();
        }

        public void CloseDevices()
        {
            blync.CloseDevices( lightCount );
        }

        private void changeToGreen()
        {
            blync.TurnOnGreenLight( deviceSelection );
            CurrentColor = BlyncColor.Green;
        }

        private void changeToBlue()
        {
            blync.TurnOnBlueLight( deviceSelection );
            CurrentColor = BlyncColor.Blue;
        }

        private void changeToCyan()
        {
            blync.TurnOnCyanLight( deviceSelection );
            CurrentColor = BlyncColor.Cyan;
        }

        private void changeToWhite()
        {
            blync.TurnOnWhiteLight( 0 );
            CurrentColor = BlyncColor.White;
        }

        private void changeToYellow()
        {
            blync.TurnOnYellowLight( deviceSelection );
            CurrentColor = BlyncColor.Yellow;
        }

        private void changeToPurple()
        {
            blync.TurnOnMagentaLight( deviceSelection );
            CurrentColor = BlyncColor.Purple;
        }

        private void changeToRed()
        {
            blync.TurnOnRedLight( deviceSelection );
            CurrentColor = BlyncColor.Red;
        }

        public void ResetLight()
        {
            blync.ResetLight( deviceSelection );
            changeToGreen();
            CurrentColor = BlyncColor.Green;
        }

        public void TurnOffLight()
        {
            blync.ResetLight( deviceSelection );
        }
    }
}