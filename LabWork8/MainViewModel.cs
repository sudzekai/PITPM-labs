using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.CompilerServices;
using System.Windows;

namespace LabWork8
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private Visibility _authVisibility = Visibility.Visible;
        public Visibility AuthVisibility
        {
            get => _authVisibility;
            set
            {
                if (_authVisibility == value) return;
                _authVisibility = value;
                OnPropertyChanged();
            }
        }

        private string _login = "";
        public string Login
        {
            get => _login;
            set
            {
                if (_login == value) return;
                _login = value;
                OnPropertyChanged();
            }
        }

        private string _password = "";
        public string Password
        {
            get => _password;
            set
            {
                if (_password == value) return;
                _password = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand _authCommand;
        public RelayCommand AuthCommand
        {
            get => _authCommand ??= new RelayCommand((o) =>
            {
                if (string.IsNullOrWhiteSpace(Login))
                    MessageBox.Show("Поле логин обязательно для ввода", "Ошибка");
                else if (string.IsNullOrWhiteSpace(Password))
                    MessageBox.Show("Поле пароль обязательно для ввода", "Ошибка");

                else if (!Login.Equals("admin") && !Password.Equals("admin"))
                    MessageBox.Show("Логин и пароль введены неверно", "Ошибка");

                else if (!Login.Equals("admin"))
                    MessageBox.Show("Логин введён неверно", "Ошибка");
                else if (!Password.Equals("admin"))
                    MessageBox.Show("Пароль введён неверно", "Ошибка");
                else
                {
                    MessageBox.Show($"С возвращением {Login}", "Успех");
                    AuthVisibility = Visibility.Collapsed;
                }
            });
        }

        public void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
