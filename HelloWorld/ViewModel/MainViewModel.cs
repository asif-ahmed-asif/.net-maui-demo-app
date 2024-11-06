using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HelloWorld.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        IConnectivity connectivity;
        private string originalText;

        public MainViewModel(IConnectivity connectivity)
        {
            Items = new ObservableCollection<string>();
            this.connectivity = connectivity;
        }

        [ObservableProperty]
        ObservableCollection<string> items;

        [ObservableProperty]
        string text;

        [RelayCommand]
        async Task AddOrEdit()
        {
            if (string.IsNullOrEmpty(Text))
                return;

            Text = Text.Trim();

            if (Items.Contains(Text, StringComparer.OrdinalIgnoreCase) &&
                !string.Equals(Text, originalText, StringComparison.OrdinalIgnoreCase))
            {
                await Shell.Current.DisplayAlert("Duplicate", "This item already exists.", "OK");
                return;
            }

            if (connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("No Internet", "You need to be connected to the internet to add items.", "OK");
                return;
            }

            if (!string.IsNullOrEmpty(originalText))
            {
                var index = Items.IndexOf(originalText);
                if (index >= 0)
                {
                    Items[index] = Text;
                }
                originalText = string.Empty;
            }
            else
            {
                Items.Add(Text);
            }

            Text = string.Empty;
        }

        [RelayCommand]
        void ClearText()
        {
            Text = string.Empty;
            originalText = string.Empty;
        }

        [RelayCommand]
        void Edit(string item)
        {
            originalText = item;
            Text = item;
        }

        [RelayCommand]
        void Delete(string item)
        {
            if (items.Contains(item))
            {
                items.Remove(item);
            }
        }

        [RelayCommand]
        async Task Tap(string item)
        {
            await Shell.Current.GoToAsync($"{nameof(DetailPage)}?Id={item}");
        }
    }
}
