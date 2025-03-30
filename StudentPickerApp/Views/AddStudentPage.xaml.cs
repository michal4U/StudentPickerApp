using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using StudentPickerApp.Models;

namespace StudentPickerApp
{
    public partial class AddStudentPage : ContentPage
    {
        private List<ClassList> classLists;
        private Action saveCallback;
        private Action loadCallback;

        public AddStudentPage(List<ClassList> classLists, Action save, Action load)
        {
            InitializeComponent();
            this.classLists = classLists;
            this.saveCallback = save;
            this.loadCallback = load;
            ClassPicker.ItemsSource = classLists.Select(c => c.ClassName).ToList();
        }

        private void OnAddStudentClicked(object sender, EventArgs e)
        {
            if (ClassPicker.SelectedIndex >= 0 && !string.IsNullOrWhiteSpace(NameEntry.Text) && int.TryParse(NumberEntry.Text, out int number))
            {
                var selectedClass = classLists[ClassPicker.SelectedIndex];
                selectedClass.Students.Add(new Student { Name = NameEntry.Text, Number = number });
                saveCallback();
                loadCallback();
                Navigation.PopAsync();
            }
            else
            {
                DisplayAlert("B³¹d", "Podaj poprawne dane!", "OK");
            }
        }
    }
}
