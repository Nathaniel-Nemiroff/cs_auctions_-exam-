In order to demonstrate the time limit/item claiming functionality, there is extra functionality in the app: a fast forwarding button on the home page (in View/proj/IndexLoggedIn.cshtml) that will decement every auction listing's EndAt DateTime property by one hour, thus simulating traveling in time.

This app will also not allow users to have a negative balance, thus when an auction listing's time limit ends, instead of immediately decrementing their balance, it will allow them to claim the item via a link.  If their account balance is below 0, they will get an error message telling them they do not have enough money.




The mysql tables will have to be added by hand as well, here's some code to do that


create database csauctiondb;

create table Users (id int not null auto_increment primary key, Username varchar(30), First varchar(20), Last varchar(20), Password varchar(100), Balance int, CreatedAt timestamp not null default now(), UpdatedAt timestamp not null default now() on update now());

create table Auctions(id int not null auto_increment primary key, SellerId int, BuyerId int, Name varchar(100), Description text, Bid int, EndsAt timestamp not null default now(), CreatedAt timestamp not null default now(), UpdatedAt timestamp not null default now() on update now(), foreign key(SellerId) references Users(id), foreign key (BuyerId) references Users(id));

