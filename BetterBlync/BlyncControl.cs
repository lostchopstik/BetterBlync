using Blynclight;
using System;

namespace BetterBlync
{
    public class BlyncControl : IDisposable
    {
        private BlynclightController blync = new BlynclightController();
        private int lightCount;

        public BlyncColor CurrentColor { get; private set; }

        public enum BlyncColor
        {
            Green,
            Blue,
            Cyan,
            Magenta,
            Yellow,
            Red,
            White,
            None
        }

        public BlyncControl()
        {
            lightCount = FindBlyncLights();
            if ( lightCount == 0 )
            {
                // TODO create custom exception
                throw new System.Exception( "Could not find any Blync lights." );
            }

            // Default color is green
            changeToGreen();
            CurrentColor = BlyncColor.Green;
            blync.CloseDevices( lightCount );
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

                case BlyncColor.Magenta:
                    changeToMagenta();
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

            blync.CloseDevices( lightCount );
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
            {
                if ( CurrentColor != BlyncColor.Green )
                    blync.TurnOnGreenLight( i );
            }

            CurrentColor = BlyncColor.Green;
        }

        private void changeToBlue()
        {
            for ( int i = lightCount; i >= 0; i-- )
            {
                if ( CurrentColor != BlyncColor.Blue )
                    blync.TurnOnBlueLight( i );
            }

            CurrentColor = BlyncColor.Blue;
        }

        private void changeToCyan()
        {
            for ( int i = lightCount; i >= 0; i-- )
            {
                if ( CurrentColor != BlyncColor.Cyan )
                    blync.TurnOnCyanLight( i );
            }

            CurrentColor = BlyncColor.Cyan;
        }

        private void changeToWhite()
        {
            for ( int i = lightCount; i >= 0; i-- )
            {
                if ( CurrentColor != BlyncColor.White )
                    blync.TurnOnWhiteLight( i );
            }

            CurrentColor = BlyncColor.White;
        }

        private void changeToYellow()
        {
            for ( int i = lightCount; i >= 0; i-- )
            {
                if ( CurrentColor != BlyncColor.Yellow )
                    blync.TurnOnYellowLight( i );
            }

            CurrentColor = BlyncColor.Yellow;
        }

        private void changeToMagenta()
        {
            for ( int i = lightCount; i >= 0; i-- )
            {
                if ( CurrentColor != BlyncColor.Magenta )
                    blync.TurnOnMagentaLight( i );
            }

            CurrentColor = BlyncColor.Magenta;
        }

        private void changeToRed()
        {
            for ( int i = lightCount; i >= 0; i-- )
            {
                if ( CurrentColor != BlyncColor.Red )
                    blync.TurnOnRedLight( i );
            }

            CurrentColor = BlyncColor.Red;
        }

        public void ResetLight()
        {
            for ( int i = lightCount; i >= 0; i-- )
                blync.ResetLight( i );

            CurrentColor = BlyncColor.None;
            changeToGreen();
            blync.CloseDevices( lightCount );
        }

        public void TurnOffLight()
        {
            for ( int i = lightCount; i >= 0; i-- )
                blync.ResetLight( i );

            CurrentColor = BlyncColor.None;
            blync.CloseDevices( lightCount );
        }

        public void Dispose()
        {
            blync = null;
            lightCount = 0;
        }
    }
}