namespace HostfileManager.UI.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// The <see cref="EditableTextBlock"/> control
    /// offers functionality for in-place editing
    /// of <see cref="TextBlock"/> control instances.
    /// </summary>
    public partial class EditableTextBlock
    {
        #region public fields

        /// <summary>
        /// The dependency property that gets or sets the text of this <see cref="EditableTextBlock"/> control instance.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(EditableTextBlock));

        /// <summary>
        /// The dependency property that gets or sets the background-text of this <see cref="EditableTextBlock"/> control instance.
        /// </summary>
        public static readonly DependencyProperty BackgroundTextProperty = DependencyProperty.Register(
            "BackgroundText",
            typeof(string),
            typeof(EditableTextBlock));

        /// <summary>
        /// The dependency property that gets or sets the placeholder-text of this <see cref="EditableTextBlock"/> control instance.
        /// </summary>
        public static readonly DependencyProperty PlaceholderTextProperty = DependencyProperty.Register(
            "PlaceholderText",
            typeof(string),
            typeof(EditableTextBlock));

        /// <summary>The dependency property that gets or sets the edit mode of this <see cref="EditableTextBlock"/> control instance.</summary>
        public static readonly DependencyProperty IsInEditModeProperty = DependencyProperty.Register(
            "IsInEditMode",
            typeof(bool),
            typeof(EditableTextBlock),
            new PropertyMetadata(false));

        #endregion

        #region private fields

        /// <summary>The previous <see cref="EditableTextBlock"/> text.</summary>
        private string previousText;

        #endregion

        #region constructor

        /// <summary>Initializes a new instance of the <see cref="EditableTextBlock"/> class.</summary>
        public EditableTextBlock()
        {
            InitializeComponent();
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the text of this <see cref="EditableTextBlock"/> control instance.
        /// </summary>
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }

            set
            {
                SetValue(TextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the background-text of this <see cref="EditableTextBlock"/> control instance.
        /// </summary>
        public string BackgroundText
        {
            get { return (string)GetValue(BackgroundTextProperty); }
            set { SetValue(BackgroundTextProperty, value); }
        }

        /// <summary>
        /// Gets or sets the placeholder-text of this <see cref="EditableTextBlock"/> control instance.
        /// </summary>
        public string PlaceholderText
        {
            get { return (string)GetValue(PlaceholderTextProperty); }
            set { SetValue(PlaceholderTextProperty, value); }
        }

        /// <summary>Gets or sets a value indicating whether this <see cref="EditableTextBlock"/> control instance is in edit-mode.</summary>
        public bool IsInEditMode
        {
            get
            {
                return (bool)GetValue(IsInEditModeProperty);
            }

            set
            {
                /* save current text */
                if (value)
                {
                    this.previousText = this.Text;
                }

                /* update visibility */
                if (value)
                {
                    txtBlock.Visibility = Visibility.Collapsed;
                    txtBox.Visibility = Visibility.Visible;
                }
                else
                {
                    txtBlock.Visibility = Visibility.Visible;
                    txtBox.Visibility = Visibility.Collapsed;
                }

                /* set new value */
                SetValue(IsInEditModeProperty, value);
            }
        }

        #endregion

        #region ui events

        /// <summary>Keyboard event handler.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="KeyEventArgs"/> that contains the event data.</param>
        private void TxtBoxKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    {
                        /* commit changes and switch back to read mode */
                        this.previousText = this.Text;
                        this.IsInEditMode = false;
                    }

                    break;

                case Key.Escape:
                    {
                        /* discard changes and switch back to read mode */
                        this.Text = this.previousText;
                        this.IsInEditMode = false;
                    }

                    break;
            }
        }

        /// <summary>On Double-Click event handler.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="KeyEventArgs"/> that contains the event data.</param>
        private void TxtBlockMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.IsInEditMode && e.ClickCount >= 2)
            {
                this.IsInEditMode = true;
            }
        }

        #endregion
    }
}
