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
        int.TryParse(EntryDiscGrade.Text, out int grade);
        string control = PickerControl.SelectedItem?.ToString() ?? "Залік";
        string date = EntryDiscDate.Text ?? DateTime.Now.ToShortDateString();
        _tempDisciplines.Add(new Discipline
        {
            Title = EntryDiscTitle.Text,
            Grade = grade,
            ControlType = control,
            Date = date
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
        if (int.TryParse(EntryYear.Text, out int y)) StudentData.Year = y;
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