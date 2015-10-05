HTML5拖曳多檔案上傳 : HomeController.cs & Index.cshtml
1. 預設儲存路徑: 專案資料夾下的images
2. 檔名相同時不覆寫並重新命名
3. 檔案大小限制在: Web.config: <add key="MaxUploadSize" value="100"/>
   Index.html
4. 檔案大小在前端Index.html與後端HomeController.Upload()雙驗證
5. CSS和JS都寫在Index.cshtml