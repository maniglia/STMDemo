using STMDemo.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace STMDemo.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}