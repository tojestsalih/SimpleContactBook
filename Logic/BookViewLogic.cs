﻿using System;
using System.Linq;
using System.Windows.Input;
using FinalProjectSalihOzturk.DataAccess;
using FinalProjectSalihOzturk.Helpers;

namespace FinalProjectSalihOzturk.Logic
{
    public class BookViewLogic:ObservableObject
    {
        private readonly IContactDal _dataAccess;

        private ContactsViewLogic _contactsViewLogic;

        private string _searchText;
        public ICommand LoadContactsCommand { get; private set; }

        public ICommand LoadThisWeeksBirthdaysCommand { get; private set; }
        public ICommand LoadSearchedCommand { get; private set; }


        public BookViewLogic(IContactDal dataAccess)
        {
            this._dataAccess = dataAccess;

            ContactsViewLogic = new ContactsViewLogic(dataAccess);

            LoadContactsCommand = new RelayCommand(LoadContacts);
            LoadSearchedCommand = new RelayCommand(LoadFilteredContacts);
            LoadThisWeeksBirthdaysCommand = new RelayCommand(LoadThisWeeksBirthdays);

        }

        

        private void LoadFilteredContacts()
        {


            if (SearchText != null)
            {
                var searchResult = _dataAccess.GetContacts()
                    .Where(c => c.Name != null && c.Name.ToUpper().StartsWith(SearchText.ToUpper())
                                || c.Surname != null && c.Surname.ToUpper().StartsWith(SearchText.ToUpper())
                                || c.Email!=null && c.Email.ToUpper().Contains(SearchText.ToUpper())
                                || c.PhoneNumber !=null && c.PhoneNumber.Contains(SearchText)
                                || c.Age.ToString().Equals(SearchText)
                            || c.DateOfBirth != null && c.DateOfBirth.ToString().Contains(SearchText)
                    );

                ContactsViewLogic.GetContacts(searchResult);

            }
        }

        private void LoadContacts()
        {
            ContactsViewLogic.GetContacts(_dataAccess.GetContacts());
        }
        private void LoadThisWeeksBirthdays()
        {
            var birthdaysInWeek = _dataAccess.GetContacts().Where(c => IsDateInOneWeek(c.DateOfBirth));
            ContactsViewLogic.GetBirthdays(birthdaysInWeek);
        }

        public ContactsViewLogic ContactsViewLogic
        {
            get { return _contactsViewLogic; }
            set { OnPropertyChanged(ref _contactsViewLogic, value); }
        }
        
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;

                OnPropertyChanged("SearchText");

            }
        }
        private bool IsDateInOneWeek(DateTime date)
        {
            DateTime today = DateTime.Now;
            var weekLater = today.AddDays(7);

            if (date.Month == today.Month)
            {
                if ((weekLater.Day >= date.Day) && (today.Day <= date.Day))
                {
                    return true;
                }
            }

            return false;

        }

    }
}