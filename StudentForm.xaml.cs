using System.Collections.ObjectModel;

namespace lab3;

public partial class StudentForm : ContentPage
{
    public Student StudentData { get; private set; }
    public bool IsSaveClicked { get; private set; } = false;
    private ObservableCollection<Discipline> _tempDisciplines;

    public StudentForm()
    {
        InitializeComponent();
        StudentData = new Student();
        _tempDisciplines = new ObservableCollection<Discipline>();
        ListDisciplines.ItemsSource = _tempDisciplines;
    }
    public StudentForm(Student s) : this()
    {
        EntryName.Text = s.Name;
        EntryFaculty.Text = s.Faculty;
        EntryDepartment.Text = s.Department;
        EntryYear.Text = s.Year.ToString();
        foreach (var d in s.Disciplines)
        {
            _tempDisciplines.Add(new Discipline
            {
                Title = d.Title,
                Grade = d.Grade,
                ControlType = d.ControlType,
                Date = d.Date
            });
        }
    }
    private async void OnAddDiscClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EntryDiscTitle.Text))
        {
            await DisplayAlert("Помилка", "Введіть назву предмету", "OK");
            return;
        }

        if (!int.TryParse(EntryDiscGrade.Text, out int grade) || grade < 0 || grade > 100)
        {
            await DisplayAlert("Помилка", "Бал має бути числом від 0 до 100!", "OK");
            return;
        }

        string dateString = EntryDiscDate.Text;
        if (string.IsNullOrWhiteSpace(dateString))
        {
            dateString = DateTime.Now.ToString("dd.MM.yyyy");
        }
        else
        {
            if (!DateTime.TryParse(dateString, out DateTime tempDate))
            {
                await DisplayAlert("Помилка", "Некоректна дата! Введіть у форматі дд.мм.рррр", "OK");
                return;
            }
            dateString = tempDate.ToString("dd.MM.yyyy");
        }

        string control = PickerControl.SelectedItem?.ToString() ?? "Залік";

        _tempDisciplines.Add(new Discipline
        {
            Title = EntryDiscTitle.Text,
            Grade = grade,
            ControlType = control,
            Date = dateString
        });
        EntryDiscTitle.Text = "";
        EntryDiscGrade.Text = "";
        EntryDiscDate.Text = "";
        PickerControl.SelectedIndex = -1;
    }
    private void OnDeleteDiscClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var discipline = button.CommandParameter as Discipline;
        if (discipline != null)
        {
            _tempDisciplines.Remove(discipline);
        }
    }
    private void OnEditDiscClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var discipline = button.CommandParameter as Discipline;
        if (discipline != null)
        {
            EntryDiscTitle.Text = discipline.Title;
            EntryDiscGrade.Text = discipline.Grade.ToString();
            EntryDiscDate.Text = discipline.Date;
            PickerControl.SelectedItem = discipline.ControlType;
            _tempDisciplines.Remove(discipline);
        }
    }
    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EntryName.Text))
        {
            await DisplayAlert("Помилка", "Введіть ПІБ студента", "OK");
            return;
        }
        StudentData.Name = EntryName.Text;
        StudentData.Faculty = EntryFaculty.Text;
        StudentData.Department = EntryDepartment.Text;
        if (int.TryParse(EntryYear.Text, out int y))
        {
            if (y < 1 || y > 6)
            {
                await DisplayAlert("Помилка", "Курс має бути від 1 до 6", "OK");
                return;
            }
            StudentData.Year = y;
        }
        else
        {
            await DisplayAlert("Помилка", "Курс має бути числом", "OK");
            return;
        }
        StudentData.Disciplines = _tempDisciplines.ToList();
        IsSaveClicked = true;
        await Navigation.PopModalAsync();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        IsSaveClicked = false;
        await Navigation.PopModalAsync();
    }
}