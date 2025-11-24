using System.Collections.ObjectModel;

namespace lab3;

public partial class MainPage : ContentPage
{
    private readonly JsonManager _jsonManager = new();
    private readonly StudentManager _studentManager = new();

    private string _currentFilePath;
    private ObservableCollection<string> _facultiesList = new();

    public MainPage()
    {
        InitializeComponent();
        PickerSearchFaculty.ItemsSource = _facultiesList;
    }
    private async void BtnClickLoadJson(object sender, EventArgs e)
    {
        try
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Оберіть JSON файл",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.WinUI, new[] { ".json" } },
                    { DevicePlatform.Android, new[] { "application/json" } }
                })
            });

            if (result != null)
            {
                _currentFilePath = result.FullPath;
                var list = await _jsonManager.LoadFromJson(_currentFilePath);

                _studentManager.LoadStudents(list);
                RefreshData();
                LblStatus.Text = $"Файл: {result.FileName}";
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка", $"Не вдалося відкрити файл:\n{ex.Message}", "OK");
        }
    }
    private async void BtnClickSaveJson(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_currentFilePath))
        {
            await DisplayAlert("Увага", "Спочатку відкрийте файл!", "OK");
            return;
        }
        try
        {
            await _jsonManager.SaveToJson(_currentFilePath, _studentManager.GetAllStudents());
            await DisplayAlert("Успіх", "Дані успішно збережено!", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка", $"Не вдалося зберегти:\n{ex.Message}", "OK");
        }
    }
    private void BtnClickSearch(object sender, EventArgs e)
    {
        int? year = int.TryParse(EntrySearchYear.Text, out int y) ? y : null;
        string faculty = PickerSearchFaculty.SelectedIndex > -1 ? PickerSearchFaculty.SelectedItem.ToString() : null;

        var searchParams = new SearchParams
        {
            Name = EntrySearchName.Text,
            Faculty = faculty,
            Department = EntrySearchDept.Text,
            DisciplineName = EntrySearchDisc.Text,
            Year = year
        };

        var results = _studentManager.Search(searchParams);
        UpdateListUI(results);

        if (results.Count == 0)
            DisplayAlert("Пошук", "За вашим запитом нічого не знайдено", "OK");
    }

    private void BtnClickClear(object sender, EventArgs e)
    {
        EntrySearchName.Text = string.Empty;
        EntrySearchYear.Text = string.Empty;
        EntrySearchDept.Text = string.Empty;
        EntrySearchDisc.Text = string.Empty;
        PickerSearchFaculty.SelectedIndex = -1;
        _studentManager.LoadStudents(new List<Student>());

        _currentFilePath = null;

        _facultiesList.Clear();
        UpdateListUI(null);
        LblStatus.Text = "Файл не обрано";
    }
    private async void BtnClickAdd(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_currentFilePath))
        {
            await DisplayAlert("Помилка", "Спочатку відкрийте файл JSON, щоб додавати студентів!", "OK");
            return;
        }

        var form = new StudentForm();
        await Navigation.PushModalAsync(form);

        form.Unloaded += (s, args) =>
        {
            if (form.IsSaveClicked)
            {
                _studentManager.AddStudent(form.StudentData);
                RefreshData();
            }
        };
    }
    private async void OnEditStudentClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var student = button.CommandParameter as Student;

        if (student == null) return;

        var form = new StudentForm(student);
        await Navigation.PushModalAsync(form);

        form.Unloaded += (s, args) =>
        {
            if (form.IsSaveClicked)
            {
                _studentManager.UpdateStudent(student, form.StudentData);
                RefreshData();
            }
        };
    }
    private async void OnDeleteStudentClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var student = button.CommandParameter as Student;

        if (student == null) return;

        bool answer = await DisplayAlert("Видалення", $"Видалити студента {student.Name}?", "Так", "Ні");
        if (answer)
        {
            _studentManager.DeleteStudent(student);
            RefreshData();
        }
    }
    private void UpdateListUI(List<Student> students)
    {
        StudentsList.ItemsSource = null;
        StudentsList.ItemsSource = students;
    }
    private void RefreshData()
    {
        UpdateListUI(_studentManager.GetAllStudents());
        UpdateFilters();
    }
    private void UpdateFilters()
    {
        var currentSelection = PickerSearchFaculty.SelectedItem;
        _facultiesList.Clear();
        _facultiesList.Add("Всі факультети");

        foreach (var f in _studentManager.GetUniqueFaculties())
        {
            _facultiesList.Add(f);
        }

        if (currentSelection != null && _facultiesList.Contains(currentSelection))
        {
            PickerSearchFaculty.SelectedItem = currentSelection;
        }
    }
    private async void BtnClickAbout(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new AboutPage());
    }
    private async void BtnClickExit(object sender, EventArgs e)
    {
        bool exit = await DisplayAlert("Вихід", "Ви точно хочете вийти?", "Так", "Ні");
        if (exit)
        {
            Application.Current.Quit();
        }
    }
}