﻿using Sneakerhead.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sneakerhead.Views
{
    /// <summary>
    /// Логика взаимодействия для UserLikesPage.xaml
    /// </summary>
    public partial class UserLikesPage : Page
    {
        public UserLikesPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            using (UserContext db = new UserContext())
            {
                MainWindow window = Window.GetWindow(this) as MainWindow;
                User thisUser = window.getCurrentUser();
                db.Users.Load();
                User user = db.Users.Local.ToBindingList().Where(u => u.ID == thisUser.ID).First();

                db.Sneakers.Load();
                var sneakers = db.Sneakers.Include(u => u.User).ToList();
                sneakersGrid.ItemsSource = sneakers.Where(u => u.User.Contains(user));
            }
        }

        private void NewOrder_Click(object sender, RoutedEventArgs e)
        {
            Button cmd = (Button)sender;
            if (cmd.DataContext is Sneaker)
            {
                MainWindow window = Window.GetWindow(this) as MainWindow;
                window.setCurrentSneaker((Sneaker)cmd.DataContext);
                UserNewOrderPage userNewOrderPage = new UserNewOrderPage();
                NavigationService.Navigate(userNewOrderPage);
            }
        }

        private void RemoveSneaker_Click(object sender, RoutedEventArgs e)
        {
            using (UserContext db = new UserContext())
            {
                Button cmd = (Button)sender;
                if (cmd.DataContext is Sneaker)
                {
                    MainWindow window = Window.GetWindow(this) as MainWindow;
                    User thisUser = window.getCurrentUser();
                    Sneaker thisSneaker = (Sneaker)cmd.DataContext;

                    db.Users.Load();
                    //User user = db.Users.Local.ToBindingList().Where(u => u.ID == thisUser.ID).First();
                    User user = db.Users.Include(u => u.Sneaker).First(u => u.ID == thisUser.ID);

                    db.Sneakers.Load();
                    //Sneaker sneaker = db.Sneakers.Local.ToBindingList().Where(u => u.ID == thisSneaker.ID).First();
                    Sneaker sneaker = db.Sneakers.Include(u => u.User).First(u => u.ID == thisSneaker.ID);

                    user.Sneaker.Remove(sneaker);
                    db.SaveChanges();

                    sneakersGrid.ItemsSource = null;
                    db.Sneakers.Load();
                    var sneakers = db.Sneakers.Include(u => u.User).ToList();
                    sneakersGrid.ItemsSource = sneakers.Where(u => u.User.Contains(user));
                }
            }
        }
    }
}
