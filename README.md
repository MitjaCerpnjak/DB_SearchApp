# DB_SearchApp
A simple C# WPF application to search through a local database (LiteDB - NoSQL) with addition of add/remove and import/export data option.

Requirements:

-Visual Studio, which can be downloaded at https://www.visualstudio.com/downloads/

-LiteDB packages installed in Visual Studio under Tools - NuGet Package Manager - Package Manager Console by running "Install-Package LiteDB". Additional information about LiteDB at http://www.litedb.org/

How it works:
The TextBoxes (Customer Name, Type of Battery, Date of Purchase) allow you to Search for or Insert of customers.
Search can be done with the input of data into one or multiple TextBoxes and pressing the Search button, 
whereas Insert needs all TextBoxes filled to work.

Show All button shows all data in the DataBase and can be used after you've searched for some specific data. 

To Remove a customer you simply double-click the customer and click the Remove button.

Import currently only works with .txt files. The function LiteDB_CreateAndImport() reads the .txt file and inputs the data into the DataBase. Each object is split with TAB(\t) ie. ID(\t)Name(\t)Battery(\t)Date.

Export takes the data from the DataBase and writes them into a selected .txt file.
