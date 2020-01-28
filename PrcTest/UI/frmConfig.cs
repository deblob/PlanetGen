using PrcTest.Attributes;
using PrcTest.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static FastNoise;

namespace PrcTest.UI
{
    public partial class frmConfig : Form
    {
        private readonly FileVersionInfo _programVersion;

        private PlanetGenerator _generator;
        private List<ListViewItem> _allItems;

        public string OutputName
        {
            get { return txtOutputName.Text; }
        }

        public frmConfig( PlanetGenerator generator )
        {
            InitializeComponent();
            _generator = generator;
            _allItems = new List<ListViewItem>();

            _programVersion = FileVersionInfo.GetVersionInfo( Assembly.GetEntryAssembly().Location );
            this.Text = "PlanetGenerator Reborn " + _programVersion.FileVersion + " by Marc D.";
        }

        private void frmConfig_Load( object sender, EventArgs e )
        {
            lstValues.Items.Clear();

            Type generatorType = _generator.GetType();
            foreach ( var field in generatorType.GetFields().Where( f => f.Attributes.HasFlag( FieldAttributes.Public )
                  && f.CustomAttributes.Any( a => a.AttributeType == typeof( ConfigAttribute ) ) ).OrderBy( n => n.Name ) )
            {
                ListViewItem item = new ListViewItem(field.Name);
                object value = field.GetValue(_generator);
                if ( value is bool )
                    item.SubItems.Add( ( bool )value ? "Enabled" : "Disabled" );
                else
                    item.SubItems.Add( field.GetValue( _generator ).ToString() );
                item.Tag = field;

                _allItems.Add( item );
                lstValues.Items.Add( item );
            }
        }

        private void lstValues_SelectedIndexChanged( object sender, EventArgs e )
        {
            if ( lstValues.SelectedItems.Count != 1 )
            {
                lblDescription.Text = String.Empty;
                pnlContainer.Controls.Clear();
                btnApplyValue.Visible = false;
                btnAnimate.Visible = false;
                return;
            }

            var selected = lstValues.SelectedItems[0];
            var fieldInfo = selected.Tag as FieldInfo;
            var configAtt = fieldInfo.GetCustomAttribute<ConfigAttribute>();

            if ( !String.IsNullOrEmpty( configAtt.Description ) )
                lblDescription.Text = configAtt.Description;
            else
                lblDescription.Text = "No description";

            var value = fieldInfo.GetValue(_generator);
            Control controller = null;
            if ( fieldInfo.FieldType == typeof( Int32 ) )
            {
                controller = new ctrlIntController( configAtt.IntFrom, configAtt.IntTo );
                ( controller as ctrlIntController ).Value = ( int )value;
            }
            else if ( fieldInfo.FieldType == typeof( Boolean ) )
            {
                controller = new ctrlBoolController();
                ( controller as ctrlBoolController ).Value = ( bool )value;
            }
            else if ( fieldInfo.FieldType == typeof( Single ) )
            {
                controller = new ctrlFloatController( configAtt.FloatFrom, configAtt.FloatTo );
                ( controller as ctrlFloatController ).Value = ( float )value;
            }
            else if ( fieldInfo.FieldType == typeof( NoiseType ) )
            {
                controller = new ctrlEnumController();
                ( controller as ctrlEnumController ).Value = ( NoiseType )value;
            }
            else
            {
                btnApplyValue.Visible = false;
                pnlContainer.Controls.Clear();
                return;
            }

            btnApplyValue.Visible = true;
            btnAnimate.Visible = true;
            pnlContainer.Controls.Clear();
            pnlContainer.Controls.Add( controller );
        }

        private void btnApplyValue_Click( object sender, EventArgs e )
        {
            if ( lstValues.SelectedItems.Count != 1 )
                return;

            var selected = lstValues.SelectedItems[0];
            var fieldInfo = selected.Tag as FieldInfo;

            object value = null;
            if ( fieldInfo.FieldType == typeof( Int32 ) )
            {
                value = pnlContainer.Controls.Cast<ctrlIntController>().First().Value;
                fieldInfo.SetValue( _generator, ( int )value );
            }
            else if ( fieldInfo.FieldType == typeof( Boolean ) )
            {
                value = pnlContainer.Controls.Cast<ctrlBoolController>().First().Value;
                fieldInfo.SetValue( _generator, ( bool )value );
            }
            else if ( fieldInfo.FieldType == typeof( Single ) )
            {
                value = pnlContainer.Controls.Cast<ctrlFloatController>().First().Value;
                fieldInfo.SetValue( _generator, ( float )value );
            }
            else if ( fieldInfo.FieldType == typeof( NoiseType ) )
            {
                value = pnlContainer.Controls.Cast<ctrlEnumController>().First().Value;
                fieldInfo.SetValue( _generator, ( Enum )value );
            }

            if ( value is bool )
                selected.SubItems[ 1 ].Text = ( bool )value ? "Enabled" : "Disabled";
            else
                selected.SubItems[ 1 ].Text = value.ToString();
        }

        private void btnOk_Click( object sender, EventArgs e )
        {
            if ( _animator != null )
            {
                if ( _generator.ALL_RANDOM_SEED == -1 )
                {
                    if ( MessageBox.Show( "The RANDOM_SEED property is set to -1. This will produce a different landscape for every frame. Continue?", "Warning",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Information ) == DialogResult.No )
                        return;
                }

                _animator.AnimateTest( txtOutputName.Text );
                return;
            }
            int lastSeed = _generator.GeneratePlanet(txtOutputName.Text);
            MessageBox.Show( "Finished generating.\nSeed used: " + lastSeed, "Done", MessageBoxButtons.OK, MessageBoxIcon.Information );
            lblLastSeed.Text = "Last seed: " + lastSeed;
        }

        private AnimationController _animator;
        private void btnAnimate_Click( object sender, EventArgs e )
        {
            if ( lstValues.SelectedIndices.Count != 1 )
                return;

            using ( Form frmAnimate = new Form() )
            {
                frmAnimate.Text = "Animate value";
                frmAnimate.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                frmAnimate.StartPosition = FormStartPosition.CenterParent;

                Type controlType = pnlContainer.Controls.Cast<Control>().First().GetType();
                Control ctrlStart = (Control)Activator.CreateInstance(controlType);
                Control ctrlEnd = (Control)Activator.CreateInstance(controlType);

                ctrlStart.Location = new Point( 0, 0 );
                ctrlEnd.Location = new Point( 0, ctrlStart.Height + 10 );

                Button btnOk = new Button() { Text = "Ok", Width = 32, Height = 23 };
                btnOk.Location = new Point( frmAnimate.Width - btnOk.Width - 80, ctrlStart.Height + ctrlEnd.Height + 20 );

                btnOk.Click += ( s, f ) =>
                {
                    frmAnimate.DialogResult = DialogResult.OK;
                    frmAnimate.Close();
                };

                frmAnimate.Controls.Add( btnOk );

                frmAnimate.Width = ctrlStart.Width + 20;
                frmAnimate.Height = ctrlStart.Height + ctrlEnd.Height + 85;

                frmAnimate.Controls.Add( ctrlStart );
                frmAnimate.Controls.Add( ctrlEnd );

                if ( frmAnimate.ShowDialog() != DialogResult.OK )
                    return;

                if ( _animator == null )
                {
                    _animator = new AnimationController( _generator );
                    btnResetAnimations.Visible = true;
                }

                if ( ctrlStart is ctrlFloatController )
                {
                    float from = ((ctrlFloatController)ctrlStart).Value;
                    float to = ((ctrlFloatController)ctrlEnd).Value;
                    _animator.AddAnimation( lstValues.SelectedItems[ 0 ].Tag as FieldInfo, from, to );
                    lstValues.SelectedItems[ 0 ].SubItems[ 1 ].Text = from + " - " + to;
                }
                else if ( ctrlStart is ctrlIntController )
                {
                    int from = ((ctrlIntController)ctrlStart).Value;
                    int to = ((ctrlIntController)ctrlEnd).Value;
                    _animator.AddAnimation( lstValues.SelectedItems[ 0 ].Tag as FieldInfo, from, to );
                    lstValues.SelectedItems[ 0 ].SubItems[ 1 ].Text = from + " - " + to;
                }

                lstValues.SelectedItems[ 0 ].ForeColor = Color.Brown;
            }
        }

        private void btnResetAnimations_Click( object sender, EventArgs e )
        {
            if ( _animator != null )
            {
                _animator = null;
                foreach ( var item in lstValues.Items.Cast<ListViewItem>() )
                {
                    item.ForeColor = Color.Black;
                    item.SubItems[ 1 ].Text = ( item.Tag as FieldInfo ).GetValue( _generator ).ToString();
                }
                btnResetAnimations.Visible = false;
            }
        }

        private void txtSearch_TextChanged( object sender, EventArgs e )
        {
            if ( !String.IsNullOrEmpty( txtSearch.Text ) )
            {
                var keywords = txtSearch.Text.Split(' ', ',', ';').Select(k => k.ToLower());

                var filtered = _allItems.Where(i =>
                {
                    bool result = true;
                    foreach (string keyword in keywords)
                        if (!i.SubItems[0].Text.ToLower().Contains(keyword))
                            result = false;
                    return result;
                }).ToArray();
                lstValues.Items.Clear();
                lstValues.Items.AddRange( filtered );
            }
            else
            {
                lstValues.Items.Clear();
                lstValues.Items.AddRange( _allItems.ToArray() );
            }
        }

        private void btnSaveSettings_Click( object sender, EventArgs e )
        {
            XElement root = new XElement( "PlanetGenSettings" );
            root.SetAttributeValue( "GeneratedWithVersion", _programVersion.FileVersion );

            Type generatorType = _generator.GetType();
            foreach ( var field in generatorType.GetFields().Where( f => f.Attributes.HasFlag( FieldAttributes.Public )
                  && f.CustomAttributes.Any( a => a.AttributeType == typeof( ConfigAttribute ) ) ) )
            {
                string elementType = "null";
                string elementValue = "null";
                if ( field.FieldType == typeof( Int32 ) )
                {
                    elementType = "Int32";
                    elementValue = field.GetValue( _generator ).ToString();
                }
                else if ( field.FieldType == typeof( Boolean ) )
                {
                    elementType = "Bool";
                    elementValue = field.GetValue( _generator ).ToString();
                }
                else if ( field.FieldType == typeof( Single ) )
                {
                    elementType = "Float";
                    elementValue = field.GetValue( _generator ).ToString();
                }
                else if ( field.FieldType == typeof( NoiseType ) )
                {
                    elementType = "EnumNoiseType";
                    elementValue = field.GetValue( _generator ).ToString();
                }


                XElement element = new XElement( elementType );
                element.SetAttributeValue( "Name", field.Name );
                element.SetAttributeValue( "Value", elementValue );
                root.Add( element );
            }

            using ( SaveFileDialog dialog = new SaveFileDialog() )
            {
                dialog.Title = "Select location to save settings to";
                dialog.InitialDirectory = Environment.CurrentDirectory;
                dialog.FileName = "settings.xml";
                dialog.Filter = "XML file|*.xml|All files|*.*";

                if ( dialog.ShowDialog() == DialogResult.Cancel )
                    return;

                root.Save( dialog.FileName );
            }
        }

        private void btnLoadSettings_Click( object sender, EventArgs e )
        {
            XElement root = null;

            using ( OpenFileDialog dialog = new OpenFileDialog() )
            {
                dialog.Title = "Select settings to load";
                dialog.InitialDirectory = Directory.Exists( Path.Combine( Environment.CurrentDirectory, "presets" ) )
                    ? Path.Combine( Environment.CurrentDirectory, "presets" )
                    : Environment.CurrentDirectory;
                dialog.Filter = "XML files|*.xml|All files|*.*";
                dialog.Multiselect = false;

                if ( dialog.ShowDialog() == DialogResult.Cancel )
                    return;

                root = XElement.Load( dialog.FileName );
            }

            var versionString = root.Attribute( "GeneratedWithVersion" ).Value;
            int major = Int32.Parse( versionString.Split( '.' )[0] );
            int minor = Int32.Parse( versionString.Split( '.' )[1] );
            int build = Int32.Parse( versionString.Split( '.' )[2] );

            if ( _programVersion.FileMajorPart > major
                || ( !( _programVersion.FileMajorPart > major ) && _programVersion.FileMinorPart > minor )
                || ( !( _programVersion.FileMajorPart > major ) && !( _programVersion.FileMinorPart > minor ) && _programVersion.FileBuildPart > build ) )
            {
                MessageBox.Show( $"These settings were generated with an older version of this program ({major}.{minor}.{build}.X). Some settings may be ignored.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning );
            }

            //Type generatorType = _generator.GetType();
            //foreach ( XElement element in root.Elements() )
            //{
            //    Type elementType = null;
            //    switch ( element.Name.ToString() )
            //    {
            //        case "Int32":
            //            elementType = typeof( Int32 );
            //            break;
            //        case "Bool":
            //            elementType = typeof( Boolean );
            //            break;
            //        case "Float":
            //            elementType = typeof( Single );
            //            break;
            //        case "EnumNoiseType":
            //            elementType = typeof( NoiseType );
            //            break;
            //    }

            //    string elementName = element.Attribute( "Name" ).Value;
            //    string elementValue = element.Attribute( "Value" ).Value;

            //    if ( elementType != typeof( NoiseType ) )
            //    {
            //        generatorType.GetField( elementName ).SetValue( _generator, Convert.ChangeType( elementValue, elementType ) );
            //    }
            //    else
            //    {
            //        generatorType.GetField( elementName ).SetValue( _generator, Enum.Parse( typeof( NoiseType ), elementValue ) );
            //    }
            //}

            _generator.LoadSettings( root );

            lstValues.SelectedIndices.Clear();
            lstValues.SelectedItems.Clear();
            lstValues_SelectedIndexChanged( this, null );
            frmConfig_Load( this, null );
        }

        private void lblLastSeed_Click( object sender, EventArgs e )
        {
            if ( lblLastSeed.Text.Contains( "N/A" ) )
                return;
            if ( MessageBox.Show( "Overwrite current seed?", "Change seed", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.No )
                return;

             _generator.GetType()
                .GetField( "ALL_RANDOM_SEED" )
                .SetValue( _generator, Int32.Parse( lblLastSeed.Text.Split( ' ' )[ 2 ] ) );
            frmConfig_Load( this, null );
        }
    }
}
