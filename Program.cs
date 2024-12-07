using Väderdata.Core;
using Väderdata.UI;



ExcelData excel = new ExcelData();
excel.ReadFileAndUploadToDB(@"C:\Users\cr7gr\Desktop\ProjectData\TempFuktData.csv");
Menu.DisplayAndSelectFromMenu();
