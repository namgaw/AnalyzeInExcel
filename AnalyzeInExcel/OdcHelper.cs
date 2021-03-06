﻿using System;
using System.IO;
using System.Windows;

namespace AnalyzeInExcel
{
    public static class OdcHelper
    {
        public static string CreateOdcFile(string datasource, string database, string cube)
        {
            string oledbConnectionString = ModelHelper.GetOleDbConnectionString(datasource, database);
            string odcHeader = @"
<html xmlns:o=""urn:schemas-microsoft-com:office:office""
xmlns=""http://www.w3.org/TR/REC-html40"">

<head>
<meta http-equiv=Content-Type content=""text/x-ms-odc; charset=utf-8"">
<meta name=ProgId content=ODC.Cube>
<meta name=SourceType content=OLEDB>
<meta name=Catalog content=PRS>
<meta name=Table content=Model>
<title>Analyze in Excel for Power BI Desktop - ODC</title>
<xml id=docprops><o:DocumentProperties
  xmlns:o=""urn:schemas-microsoft-com:office:office""
  xmlns=""http://www.w3.org/TR/REC-html40"">
  <o:Name>mtbsql608v-dev_mssqlinst01 PRS Model</o:Name>
 </o:DocumentProperties>
</xml>";
            var odcBody = @"<xml id=msodc><odc:OfficeDataConnection
  xmlns:odc=""urn:schemas-microsoft-com:office:odc""
  xmlns=""http://www.w3.org/TR/REC-html40"">
  <odc:Connection odc:Type=""OLEDB"">
   <odc:ConnectionString>{0}</odc:ConnectionString>
   <odc:CommandType>Cube</odc:CommandType>
   <odc:CommandText>{1}</odc:CommandText>
  </odc:Connection>
 </odc:OfficeDataConnection>
</xml>";

            var odcFooter = @"
<style>
<!--
    .ODCDataSource
    {
    behavior: url(dataconn.htc);
    }
-->
</style>
 
</head>

<body onload='init()' scroll=no leftmargin=0 topmargin=0 rightmargin=0 style='border: 0px'>
<table style='border: solid 1px threedface; height: 100%; width: 100%' cellpadding=0 cellspacing=0 width='100%'> 
  <tr> 
    <td id=tdName style='font-family:arial; font-size:medium; padding: 3px; background-color: threedface'> 
      &nbsp; 
    </td> 
     <td id=tdTableDropdown style='padding: 3px; background-color: threedface; vertical-align: top; padding-bottom: 3px'>

      &nbsp; 
    </td> 
  </tr> 
  <tr> 
    <td id=tdDesc colspan='2' style='border-bottom: 1px threedshadow solid; font-family: Arial; font-size: 1pt; padding: 2px; background-color: threedface'>

      &nbsp; 
    </td> 
  </tr> 
  <tr> 
    <td colspan='2' style='height: 100%; padding-bottom: 4px; border-top: 1px threedhighlight solid;'> 
      <div id='pt' style='height: 100%' class='ODCDataSource'></div> 
    </td> 
  </tr> 
</table> 

  
<script language='javascript'> 

function init() { 
  var sName, sDescription; 
  var i, j; 
  
  try { 
    sName = unescape(location.href) 
  
    i = sName.lastIndexOf(""."") 
    if (i>=0) { sName = sName.substring(1, i); } 
  
    i = sName.lastIndexOf(""/"") 
    if (i>=0) { sName = sName.substring(i+1, sName.length); } 

    document.title = sName; 
    document.getElementById(""tdName"").innerText = sName; 

    sDescription = document.getElementById(""docprops"").innerHTML; 
  
    i = sDescription.indexOf(""escription>"") 
    if (i>=0) { j = sDescription.indexOf(""escription>"", i + 11); } 

    if (i>=0 && j >= 0) { 
      j = sDescription.lastIndexOf(""</"", j); 

      if (j>=0) { 
          sDescription = sDescription.substring(i+11, j); 
        if (sDescription != """") { 
            document.getElementById(""tdDesc"").style.fontSize=""x-small""; 
          document.getElementById(""tdDesc"").innerHTML = sDescription; 
          } 
        } 
      } 
    } 
  catch(e) { 

    } 
  } 
</script> 

</body> 
 
</html>

";

            var odcPath = GetOdcFilePath();
            File.WriteAllText(odcPath, odcHeader + string.Format(odcBody, oledbConnectionString, cube) + odcFooter);
            return odcPath;
        }

        private static string GetOdcFilePath()
        {
            const string ODC_FILENAME = "AnalyzeInExcel.odc";
            const string MY_DATA_SOURCES = "My Data Sources"; // This is not localized - TODO find the localized version of this name

            var myDocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments, Environment.SpecialFolderOption.Create);
            var dsPath = Path.Combine(myDocs, MY_DATA_SOURCES, ODC_FILENAME);

            try
            {
                // ensure that the folder exists
                Directory.CreateDirectory(Path.GetDirectoryName(dsPath));
            }
            catch (Exception)
            {
                // If the folder is not available or accessible, try the file in MyDocuments
                dsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    ODC_FILENAME);
            }

            try
            {
                using (StreamWriter w = File.AppendText(dsPath))
                {
                    // If the file can be opened this way, it should be writable
                }
            }
            catch ( Exception )
            {
                // If the previous attempt fails, the last resort is creating the file in the TEMP directory
                dsPath = Path.GetTempFileName().Replace(".tmp", "AnalyzeInExcel.odc");
            }
            return dsPath;
        }
    }
}
