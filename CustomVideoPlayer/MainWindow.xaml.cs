using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using Windows.Media.Core;
using WinRT.Interop; // For interop calls
using Windows.Foundation; // This contains the Point struct
using System.Runtime.InteropServices;  // For DllImport



namespace CustomVideoPlayer
{
    public sealed partial class MainWindow : Window
    {
        private bool isDragging;
        private Point lastPointerPosition;

        public MainWindow()
        {
            this.InitializeComponent();

            // Remove title bar
            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(null); // No title bar

            // Handle window dragging
            this.mediaPlayerElement.PointerPressed += MediaPlayerElement_PointerPressed;
            this.mediaPlayerElement.PointerMoved += MediaPlayerElement_PointerMoved;
            this.mediaPlayerElement.PointerReleased += MediaPlayerElement_PointerReleased;

            // Load and play video
            mediaPlayerElement.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/qj0sopzku6kc1.mp4"));
            mediaPlayerElement.MediaPlayer.Play();
        }

        // Handle PointerPressed (start dragging)
        private void MediaPlayerElement_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            isDragging = true;
            lastPointerPosition = e.GetCurrentPoint(null).Position;
        }

        // Handle PointerMoved (dragging in progress)
        private void MediaPlayerElement_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (isDragging)
            {
                var currentPointerPosition = e.GetCurrentPoint(null).Position;
                var deltaX = currentPointerPosition.X - lastPointerPosition.X;
                var deltaY = currentPointerPosition.Y - lastPointerPosition.Y;

                MoveWindow((int)deltaX, (int)deltaY);

                lastPointerPosition = currentPointerPosition;
            }
        }

        // Handle PointerReleased (stop dragging)
        private void MediaPlayerElement_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            isDragging = false;
        }

        // Move the window using native interop
        private void MoveWindow(int deltaX, int deltaY)
        {
            IntPtr hwnd = WindowNative.GetWindowHandle(this);

            // Get the current window position
            RECT rect;
            if (Win32Interop.GetWindowRect(hwnd, out rect))
            {
                int width = rect.Right - rect.Left;
                int height = rect.Bottom - rect.Top;

                // Move the window based on delta
                Win32Interop.MoveWindow(hwnd, rect.Left + deltaX, rect.Top + deltaY, width, height, true);
            }
        }
    }

}


public static class Win32Interop
{
    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
}

[StructLayout(LayoutKind.Sequential)]
public struct RECT
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;
}
