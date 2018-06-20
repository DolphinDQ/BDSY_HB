using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AirDetector
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

    }
    public class I
    {
        public I()
        {
            Its = new It[]
            {
                new It(){
                    Name ="root",
                    C=new []{
                        new It(){ Name ="child1"},
                        new It(){ Name ="child2"},
                        new It(){ Name ="child3"},
                        new It(){ Name ="child4"},
                        new It(){ Name ="child5"},
                        new It(){ Name ="child6"},
                        new It(){ Name ="child7",
                            C=new []{
                                new It(){ Name ="child1"},
                                new It(){ Name ="child2"},
                                new It(){ Name ="child3"},
                                new It(){ Name ="child4"},
                                new It(){ Name ="child5"},
                                new It(){ Name ="child6"},
                                new It(){ Name ="child7"


                                },
                    }

                        },
                    }
                }
            };
        }

        public IEnumerable<It> Its { get; set; }

    }
    public class It
    {

        public string Name { get; set; }

        public It[] C { get; set; }
    }
}
