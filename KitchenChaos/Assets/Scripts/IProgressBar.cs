/*
 * Author: Bharath Kumar S
 * Date: 25-09-2024
 * Progressbar interface
 */
using System;

public interface IProgressBar {

    public event EventHandler<OnProgressChangedEventArgs> OnProgressChanged;

    public class OnProgressChangedEventArgs : EventArgs {
        public float progressNormalized;
    }
}
