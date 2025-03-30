using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.Maui.Controls;
using StudentPickerApp.Models;
namespace StudentPickerApp
{
    public partial class MainPage : ContentPage
    {
        private const string FilePath = "students.txt";
        private List<ClassList> classLists = new();
        private ClassList selectedClass;
        private int? luckyNumber = null;

        public MainPage()
        {
            InitializeComponent();
            LoadClassLists();
        }

        private void LoadClassLists()
        {
            if (File.Exists(FilePath))
            {
                try
                {
                    var json = File.ReadAllText(FilePath);
                    classLists = JsonConvert.DeserializeObject<List<ClassList>>(json) ?? new List<ClassList>();
                }
                catch (Exception)
                {
                    classLists = new List<ClassList>();
                }
            }

            classLists = classLists.Where(c => !string.IsNullOrWhiteSpace(c.ClassName)).ToList();
            UpdateClassPicker();
        }

        private void SaveClassLists()
        {
            var json = JsonConvert.SerializeObject(classLists);
            File.WriteAllText(FilePath, json);
        }

        private void UpdateClassPicker()
        {
            var validClasses = classLists.Where(c => !string.IsNullOrWhiteSpace(c.ClassName)).ToList();

            ClassPicker.ItemsSource = validClasses.Select(c => c.ClassName).ToList();

            if (validClasses.Any())
            {
                ClassPicker.SelectedIndex = 0;
                selectedClass = validClasses.First();
                StudentsListView.ItemsSource = selectedClass.Students;
            }
            else
            {
                selectedClass = null;
                StudentsListView.ItemsSource = null;
            }
        }

        private void OnClassChanged(object sender, EventArgs e)
        {
            if (ClassPicker.SelectedIndex >= 0)
            {
                selectedClass = classLists[ClassPicker.SelectedIndex];
                StudentsListView.ItemsSource = selectedClass.Students;
            }
        }

        private async void OnAddClassClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(ClassNameEntry.Text))
            {
                var newClass = new ClassList { ClassName = ClassNameEntry.Text };
                classLists.Add(newClass);
                SaveClassLists();
                LoadClassLists();
                ClassNameEntry.Text = string.Empty;
            }
            else
            {
                await DisplayAlert("Błąd", "Podaj poprawną nazwę klasy!", "OK");
            }
        }

        private void OnAddStudentClicked(object sender, EventArgs e)
        {
            if (selectedClass == null)
            {
                DisplayAlert("Błąd", "Wybierz klasę przed dodaniem ucznia", "OK");
                return;
            }
            Navigation.PushAsync(new AddStudentPage(classLists, SaveClassLists, LoadClassLists));
        }

        private void OnLuckyNumberChanged(object sender, EventArgs e)
        {
            luckyNumber = int.TryParse(LuckyNumberEntry.Text, out int number) ? number : null;
        }

        private void OnPickStudentClicked(object sender, EventArgs e)
        {
            if (selectedClass == null)
            {
                DisplayAlert("Błąd", "Wybierz klasę przed losowaniem", "OK");
                return;
            }

            var availableStudents = selectedClass.Students.Where(s => s.IsPresent && s.Number != luckyNumber).ToList();
            if (availableStudents.Any())
            {
                var random = new Random();
                var selectedStudent = availableStudents[random.Next(availableStudents.Count)];
                DisplayAlert("Wylosowany uczeń", selectedStudent.Name, "OK");
            }
            else
            {
                DisplayAlert("Brak uczniów", "Nie ma uczniów do losowania!", "OK");
            }
        }

        private async void OnRemoveStudentClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var student = button?.BindingContext as Student;

            if (student != null)
            {
                bool confirm = await DisplayAlert("Usuń ucznia", $"Czy na pewno chcesz usunąć {student.Name}?", "Tak", "Nie");
                if (confirm)
                {
                    selectedClass.Students.Remove(student);
                    SaveClassLists();
                    StudentsListView.ItemsSource = null;
                    StudentsListView.ItemsSource = selectedClass.Students;
                }
            }
        }

        private async void OnRemoveClassClicked(object sender, EventArgs e)
        {
            if (ClassPicker.SelectedIndex >= 0)
            {
                bool confirm = await DisplayAlert("Usuń klasę", "Czy na pewno chcesz usunąć tę klasę?", "Tak", "Nie");
                if (confirm)
                {
                    var classToRemove = classLists[ClassPicker.SelectedIndex];
                    classLists.Remove(classToRemove);
                    SaveClassLists();
                    LoadClassLists();
                }
            }
            else
            {
                DisplayAlert("Błąd", "Wybierz klasę do usunięcia!", "OK");
            }
        }
    }
}
