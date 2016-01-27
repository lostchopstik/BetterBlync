using Blynclight;

namespace BetterBlync
{
    public class BlyncControl
    {
        private BlynclightController blync;
        private int lightCount;

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
            lightCount = FindBlyncLights();
            if ( lightCount == 0 )
            {
                // TODO create custom exception
                throw new System.Exception( "Could not find any Blync lights." );
            }

            // Default color is green
            changeToGreen();
            CurrentColor = BlyncColor.Green;
        }

        /// <summary>
        /// Change the color of the Blync light to any of the preset values.
        /// </summary>
        /// <param name="color">Enum value of possible colors.</param>
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

        public int FindBlyncLights()
        {
            return blync.InitBlyncDevices();
        }

        public void CloseDevices()
        {
            blync.CloseDevices( lightCount );
        }

        private void changeToGreen()
        {
            for ( int i = lightCount; i >= 0; i-- )
                blync.TurnOnGreenLight( i );

            CurrentColor = BlyncColor.Green;
        }

        private void changeToBlue()
        {
            for ( int i = lightCount; i >= 0; i-- )
                blync.TurnOnBlueLight( i );

            CurrentColor = BlyncColor.Blue;
        }

        private void changeToCyan()
        {
            for ( int i = lightCount; i >= 0; i-- )
                blync.TurnOnCyanLight( i );

            CurrentColor = BlyncColor.Cyan;
        }

        private void changeToWhite()
        {
            for ( int i = lightCount; i >= 0; i-- )
                blync.TurnOnWhiteLight( 0 );

            CurrentColor = BlyncColor.White;
        }

        private void changeToYellow()
        {
            for ( int i = lightCount; i >= 0; i-- )
                blync.TurnOnYellowLight( i );

            CurrentColor = BlyncColor.Yellow;
        }

        private void changeToPurple()
        {
            for ( int i = lightCount; i >= 0; i-- )
                blync.TurnOnMagentaLight( i );

            CurrentColor = BlyncColor.Purple;
        }

        private void changeToRed()
        {
            for ( int i = lightCount; i >= 0; i-- )
                blync.TurnOnRedLight( i );

            CurrentColor = BlyncColor.Red;
        }

        public void ResetLight()
        {
            for ( int i = lightCount; i >= 0; i-- )
                blync.ResetLight( i );

            changeToGreen();
        }

        public void TurnOffLight()
        {
            for ( int i = lightCount; i >= 0; i-- )
                blync.ResetLight( i );
        }
    }
}