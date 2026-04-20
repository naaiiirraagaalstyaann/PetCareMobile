using System.Linq;

namespace PetCareMobile
{
    public partial class MainPage : ContentPage
    {
        private readonly PetCareService _service;
        private readonly List<CheckBox> _taskCheckboxes = new List<CheckBox>();

        public MainPage(PetCareService service)
        {
            InitializeComponent();
            _service = service;

            PopulateTaskChecklist();
            _service.LoadFromFile();

            chkTodayOnly.IsChecked = true;
            RefreshList();
        }

        private void PopulateTaskChecklist()
        {
            taskListStack.Children.Clear();
            _taskCheckboxes.Clear();

            foreach (TaskType taskType in Enum.GetValues(typeof(TaskType)))
            {
                var hStack = new HorizontalStackLayout { Spacing = 10 };
                var checkBox = new CheckBox { Color = Color.FromArgb("#654336"), BindingContext = taskType };
                var label = new Label { Text = taskType.ToString(), TextColor = Color.FromArgb("#654336"), VerticalOptions = LayoutOptions.Center };

                hStack.Children.Add(checkBox);
                hStack.Children.Add(label);

                _taskCheckboxes.Add(checkBox);
                taskListStack.Children.Add(hStack);
            }
        }

        private async void OnLogClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPetName.Text))
            {
                await DisplayAlert("Missing Info", "Please enter a pet name.", "OK");
                return;
            }

            var selectedTasks = _taskCheckboxes.Where(c => c.IsChecked).Select(c => (TaskType)c.BindingContext).ToList();

            if (selectedTasks.Count == 0)
            {
                await DisplayAlert("Missing Task", "Select at least one activity.", "OK");
                return;
            }

            foreach (var task in selectedTasks)
            {
                _service.AddTask(txtPetName.Text, task, txtUserName.Text);
            }

            _service.SaveToFile();

            txtPetName.Text = string.Empty;
            foreach (var cb in _taskCheckboxes) cb.IsChecked = false;

            RefreshList();
        }

        private void OnFilterChanged(object sender, CheckedChangedEventArgs e)
        {
            RefreshList();
        }

        private void RefreshList()
        {
            var allTasks = _service.GetAllTasks();
            if (allTasks == null) return;

            historyList.ItemsSource = null;

            if (chkTodayOnly.IsChecked)
            {
                historyList.ItemsSource = allTasks
                    .Where(t => t.TimeStamp.Date == DateTime.Today)
                    .ToList();
            }
            else
            {
                historyList.ItemsSource = allTasks.ToList();
            }
        }
    }
}