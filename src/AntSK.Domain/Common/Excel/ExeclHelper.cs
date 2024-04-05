using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.Streaming;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AntSK.Domain
{
    public class ExeclHelper
    {
        /// <summary>
        /// 将excel导入到datatable
        /// </summary>
        /// <param name="filePath">excel路径</param>
        /// <param name="isColumnName">第一行是否是列名</param>
        /// <returns>返回datatable</returns>
        public static DataTable ExcelToDataTable(string filePath, bool isColumnName)
        {
            DataTable dataTable = null;
            FileStream fs = null;
            DataColumn column = null;
            DataRow dataRow = null;
            IWorkbook workbook = null;
            ISheet sheet = null;
            IRow row = null;
            ICell cell = null;
            int startRow = 0;
            try
            {
                using (fs = File.OpenRead(filePath))
                {
                    // 2007版本
                    if (filePath.Contains(".xlsx"))
                        workbook = new XSSFWorkbook(fs);
                    // 2003版本
                    else if (filePath.Contains(".xls"))
                        workbook = new HSSFWorkbook(fs);

                    if (workbook != null)
                    {
                        sheet = workbook.GetSheetAt(0);//读取第一个sheet，当然也可以循环读取每个sheet
                        dataTable = new DataTable();
                        if (sheet != null)
                        {
                            int rowCount = sheet.LastRowNum;//总行数
                            if (rowCount > 0)
                            {
                                IRow firstRow = sheet.GetRow(0);//第一行
                                int cellCount = firstRow.LastCellNum;//列数

                                //构建datatable的列
                                if (isColumnName)
                                {
                                    startRow = 1;//如果第一行是列名，则从第二行开始读取
                                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                    {
                                        cell = firstRow.GetCell(i);
                                        if (cell != null)
                                        {
                                            if (cell.StringCellValue != null)
                                            {
                                                column = new DataColumn(cell.StringCellValue);
                                                dataTable.Columns.Add(column);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                    {
                                        column = new DataColumn("column" + (i + 1));
                                        dataTable.Columns.Add(column);
                                    }
                                }

                                //填充行
                                for (int i = startRow; i <= rowCount; ++i)
                                {
                                    row = sheet.GetRow(i);
                                    if (row == null) continue;

                                    dataRow = dataTable.NewRow();
                                    for (int j = row.FirstCellNum; j < cellCount; ++j)
                                    {
                                        cell = row.GetCell(j);
                                        if (cell == null)
                                        {
                                            dataRow[j] = "";
                                        }
                                        else
                                        {
                                            //CellType(Unknown = -1,Numeric = 0,String = 1,Formula = 2,Blank = 3,Boolean = 4,Error = 5,)
                                            switch (cell.CellType)
                                            {
                                                case CellType.Blank:
                                                    dataRow[j] = "";
                                                    break;
                                                case CellType.Numeric:
                                                    short format = cell.CellStyle.DataFormat;
                                                    //对时间格式（2015.12.5、2015/12/5、2015-12-5等）的处理
                                                    if (format == 14 || format == 31 || format == 57 || format == 58)
                                                        dataRow[j] = cell.DateCellValue;
                                                    else
                                                        dataRow[j] = cell.NumericCellValue;
                                                    break;
                                                case CellType.String:
                                                    dataRow[j] = cell.StringCellValue;
                                                    break;
                                            }
                                        }
                                    }
                                    dataTable.Rows.Add(dataRow);
                                }
                            }
                        }
                    }
                }
                return dataTable;
            }
            catch (Exception)
            {
                if (fs != null)
                {
                    fs.Close();
                }
                return null;
            }
        }

        /// <summary>
        /// 将excel导入到datatable
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="isColumnName">第一行是否是列名</param>
        /// <returns></returns>
        public static DataTable ExcelToDataTable(Stream stream, bool isColumnName)
        {
            DataTable dataTable = null;
            DataColumn column = null;
            DataRow dataRow = null;
            IWorkbook workbook = new XSSFWorkbook(stream);
            ISheet sheet = null;
            IRow row = null;
            ICell cell = null;
            int startRow = 0;
            try
            {

                if (workbook != null)
                {
                    sheet = workbook.GetSheetAt(0);//读取第一个sheet，当然也可以循环读取每个sheet
                    dataTable = new DataTable();
                    if (sheet != null)
                    {
                        int rowCount = sheet.LastRowNum;//总行数
                        if (rowCount > 0)
                        {
                            IRow firstRow = sheet.GetRow(0);//第一行
                            int cellCount = firstRow.LastCellNum;//列数

                            //构建datatable的列
                            if (isColumnName)
                            {
                                startRow = 1;//如果第一行是列名，则从第二行开始读取
                                for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                {
                                    cell = firstRow.GetCell(i);
                                    if (cell != null)
                                    {
                                        if (cell.StringCellValue != null)
                                        {
                                            column = new DataColumn(cell.StringCellValue);
                                            dataTable.Columns.Add(column);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                {
                                    column = new DataColumn("column" + (i + 1));
                                    dataTable.Columns.Add(column);
                                }
                            }

                            //填充行
                            for (int i = startRow; i <= rowCount; ++i)
                            {
                                row = sheet.GetRow(i);
                                if (row == null) continue;

                                dataRow = dataTable.NewRow();
                                for (int j = row.FirstCellNum; j < cellCount; ++j)
                                {
                                    cell = row.GetCell(j);
                                    if (cell == null)
                                    {
                                        dataRow[j] = "";
                                    }
                                    else
                                    {
                                        //CellType(Unknown = -1,Numeric = 0,String = 1,Formula = 2,Blank = 3,Boolean = 4,Error = 5,)
                                        switch (cell.CellType)
                                        {
                                            case CellType.Blank:
                                                dataRow[j] = "";
                                                break;
                                            case CellType.Numeric:
                                                short format = cell.CellStyle.DataFormat;
                                                //对时间格式（2015.12.5、2015/12/5、2015-12-5等）的处理
                                                if (format == 14 || format == 31 || format == 57 || format == 58)
                                                    dataRow[j] = cell.DateCellValue;
                                                else
                                                    dataRow[j] = cell.NumericCellValue;
                                                break;
                                            case CellType.String:
                                                dataRow[j] = cell.StringCellValue;
                                                break;
                                        }
                                    }
                                }
                                dataTable.Rows.Add(dataRow);
                            }
                        }
                    }
                }

                return dataTable;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// excel转list
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> ExcelToList<TResult>(Stream stream) where TResult : new()
        {
            var propertyInfos = typeof(TResult).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CustomAttributes.Count() > 0)
                .OrderBy(p => p.GetCustomAttribute<ExeclPropertyAttribute>().Order).ToArray();

            List<TResult> list = new List<TResult>();

            IWorkbook workbook = new XSSFWorkbook(stream);
            ISheet sheet = null;
            IRow row = null;
            ICell cell = null;
            int startRow = 1;
            try
            {

                if (workbook != null)
                {
                    sheet = workbook.GetSheetAt(0);//读取第一个sheet，当然也可以循环读取每个sheet
                    if (sheet != null)
                    {
                        int rowCount = sheet.LastRowNum;//总行数
                        if (rowCount > 0)
                        {
                            IRow firstRow = sheet.GetRow(0);//第一行
                            int cellCount = firstRow.LastCellNum;//列数

                            //填充行
                            for (int i = startRow; i <= rowCount; ++i)
                            {
                                row = sheet.GetRow(i);
                                if (row == null) continue;
                                bool emptyRow = true;//是否空行
                                TResult dataModel = new TResult();

                                for (int j = row.FirstCellNum; j < cellCount; ++j)
                                {
                                    var execlPropertyAttribute = propertyInfos[j].GetCustomAttribute<ExeclPropertyAttribute>();

                                    cell = row.GetCell(j);
                                    if (cell == null)
                                    {
                                        propertyInfos[j].SetValue(dataModel, "");
                                    }
                                    else
                                    {
                                        switch (cell.CellType)
                                        {
                                            case CellType.Blank:
                                                propertyInfos[j].SetValue(dataModel, "");
                                                break;
                                            case CellType.Numeric:
                                                short format = cell.CellStyle.DataFormat;
                                                //对时间格式（2015.12.5、2015/12/5、2015-12-5等）的处理
                                                if (format == 14 || format == 31 || format == 57 || format == 58)
                                                    propertyInfos[j].SetValue(dataModel, cell.DateCellValue);
                                                else
                                                {
                                                    if (execlPropertyAttribute.CellType == CellType.String)
                                                    {
                                                        propertyInfos[j].SetValue(dataModel, cell.NumericCellValue.ToString());
                                                    }
                                                    else

                                                    {
                                                        propertyInfos[j].SetValue(dataModel, cell.NumericCellValue);
                                                    }
                                                }
                                                break;
                                            case CellType.String:
                                                propertyInfos[j].SetValue(dataModel, cell.StringCellValue);
                                                break;
                                        }
                                    }

                                    if (cell != null && !string.IsNullOrEmpty(cell.ToString().Trim()))
                                    {
                                        emptyRow = false;
                                    }
                                }
                                //非空数据行数据添加到DataTable
                                if (!emptyRow)
                                {
                                    list.Add(dataModel);
                                }
                            }
                        }
                    }
                }

                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static IEnumerable<TResult> ExcelToListFileName<TResult>(Stream stream, string fileName) where TResult : new()
        {
            var propertyInfos = typeof(TResult).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CustomAttributes.Count() > 0)
                .OrderBy(p => p.GetCustomAttribute<ExeclPropertyAttribute>().Order).ToArray();

            List<TResult> list = new List<TResult>();

            IWorkbook workbook = null;
            if (fileName.Contains(".xlsx"))
                workbook = new XSSFWorkbook(stream);
            // 2003版本
            else if (fileName.Contains(".xls"))
                workbook = new HSSFWorkbook(stream);
            ISheet sheet = null;
            IRow row = null;
            ICell cell = null;
            int startRow = 1;
            try
            {

                if (workbook != null)
                {
                    sheet = workbook.GetSheetAt(0);//读取第一个sheet，当然也可以循环读取每个sheet
                    if (sheet != null)
                    {
                        int rowCount = sheet.LastRowNum;//总行数
                        if (rowCount > 0)
                        {
                            IRow firstRow = sheet.GetRow(0);//第一行
                            int cellCount = firstRow.LastCellNum;//列数

                            //填充行
                            for (int i = startRow; i <= rowCount; ++i)
                            {
                                row = sheet.GetRow(i);
                                if (row == null) continue;
                                bool emptyRow = true;//是否空行
                                TResult dataModel = new TResult();

                                for (int j = row.FirstCellNum; j < cellCount; ++j)
                                {
                                    var execlPropertyAttribute = propertyInfos[j].GetCustomAttribute<ExeclPropertyAttribute>();

                                    cell = row.GetCell(j);
                                    if (cell == null)
                                    {
                                        propertyInfos[j].SetValue(dataModel, "");
                                    }
                                    else
                                    {
                                        switch (cell.CellType)
                                        {
                                            case CellType.Blank:
                                                propertyInfos[j].SetValue(dataModel, "");
                                                break;
                                            case CellType.Numeric:
                                                short format = cell.CellStyle.DataFormat;
                                                //对时间格式（2015.12.5、2015/12/5、2015-12-5等）的处理
                                                if (format == 14 || format == 31 || format == 57 || format == 58)
                                                    propertyInfos[j].SetValue(dataModel, cell.DateCellValue);
                                                else
                                                {
                                                    if (execlPropertyAttribute.CellType == CellType.String)
                                                    {
                                                        propertyInfos[j].SetValue(dataModel, cell.NumericCellValue.ToString());
                                                    }
                                                    else

                                                    {
                                                        propertyInfos[j].SetValue(dataModel, cell.NumericCellValue);
                                                    }
                                                }
                                                break;
                                            case CellType.String:
                                                propertyInfos[j].SetValue(dataModel, cell.StringCellValue);
                                                break;
                                        }
                                    }

                                    if (cell != null && !string.IsNullOrEmpty(cell.ToString().Trim()))
                                    {
                                        emptyRow = false;
                                    }
                                }
                                //非空数据行数据添加到DataTable
                                if (!emptyRow)
                                {
                                    list.Add(dataModel);
                                }
                            }
                        }
                    }
                }

                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// excel转list-根据sheetName得到List
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="stream"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> ExcelToList<TResult>(Stream stream, string sheetName) where TResult : new()
        {
            var propertyInfos = typeof(TResult).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .OrderBy(p => p.GetCustomAttribute<ExeclPropertyAttribute>().Order).ToArray();

            List<TResult> list = new List<TResult>();

            IWorkbook workbook = new XSSFWorkbook(stream);
            ISheet sheet = null;
            IRow row = null;
            ICell cell = null;
            int startRow = 1;
            try
            {

                if (workbook != null)
                {
                    sheet = workbook.GetSheet(sheetName);//根据sheet读取对应的DataTable
                    if (sheet != null)
                    {
                        int rowCount = sheet.LastRowNum;//总行数
                        if (rowCount > 0)
                        {
                            IRow firstRow = sheet.GetRow(0);//第一行
                            int cellCount = firstRow.LastCellNum;//列数

                            //填充行
                            for (int i = startRow; i <= rowCount; ++i)
                            {
                                row = sheet.GetRow(i);
                                if (row == null) continue;
                                bool emptyRow = true;//是否空行

                                TResult dataModel = new TResult();

                                for (int j = row.FirstCellNum; j < cellCount; ++j)
                                {
                                    var execlPropertyAttribute = propertyInfos[j].GetCustomAttribute<ExeclPropertyAttribute>();

                                    cell = row.GetCell(j);
                                    if (cell == null)
                                    {
                                        propertyInfos[j].SetValue(dataModel, "");
                                    }
                                    else
                                    {
                                        switch (cell.CellType)
                                        {
                                            case CellType.Blank:
                                                propertyInfos[j].SetValue(dataModel, "");
                                                break;
                                            case CellType.Numeric:
                                                short format = cell.CellStyle.DataFormat;
                                                //对时间格式（2015.12.5、2015/12/5、2015-12-5等）的处理
                                                if (format == 14 || format == 31 || format == 57 || format == 58)
                                                    propertyInfos[j].SetValue(dataModel, cell.DateCellValue);
                                                else
                                                {
                                                    if (execlPropertyAttribute.CellType == CellType.String)
                                                    {
                                                        propertyInfos[j].SetValue(dataModel, cell.NumericCellValue.ToString());
                                                    }
                                                    else

                                                    {
                                                        propertyInfos[j].SetValue(dataModel, cell.NumericCellValue);
                                                    }

                                                }
                                                break;
                                            case CellType.String:
                                                propertyInfos[j].SetValue(dataModel, cell.StringCellValue);
                                                break;
                                        }
                                    }
                                    if (cell != null && !string.IsNullOrEmpty(cell.ToString().Trim()))
                                    {
                                        emptyRow = false;
                                    }
                                }
                                //非空数据行数据添加到DataTable
                                if (!emptyRow)
                                {
                                    list.Add(dataModel);
                                }
                            }
                        }
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// List导出excel 二进制流
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="data">List</param>
        /// <param name="sheetName">sheetname 可不填，默认Sheet0</param>
        /// <returns></returns>
        public static byte[] ListToExcel<T>(T[] data, string sheetName = "Sheet0")
        {
            IWorkbook workbook = null;
            IRow row = null;
            ISheet sheet = null;
            ICell cell = null;
            var propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                   .OrderBy(p => p.GetCustomAttribute<ExeclPropertyAttribute>().Order).ToArray();
            workbook = new XSSFWorkbook();
            sheet = workbook.CreateSheet(sheetName);//创建一个名称为Sheet0的表
            int rowCount = data.Count();//行数
            int columnCount = propertyInfos.Length;//列数
            //设置列头
            row = sheet.CreateRow(0);//excel第一行设为列头
            for (int c = 0; c < columnCount; c++)
            {
                cell = row.CreateCell(c);
                cell.SetCellValue(propertyInfos[c].GetCustomAttribute<ExeclPropertyAttribute>().DisplayName);
            }
            //设置每行每列的单元格,
            for (int i = 0; i < rowCount; i++)
            {
                row = sheet.CreateRow(i + 1);
                for (int j = 0; j < columnCount; j++)
                {
                    cell = row.CreateCell(j);//excel第二行开始写入数据
                    cell.SetCellValue(propertyInfos[j].GetValue(data[i])?.ToString());
                }
            }
            using (MemoryStream memoryStream = new MemoryStream())
            {
                workbook.Write(memoryStream);//向打开的这个xls文件中写入数据
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        ///  Dt导出excel 二进制流
        /// </summary>
        /// <param name="dt">datatable</param>
        /// <param name="strFile">strFile</param>
        /// <returns></returns>
        public static byte[] DataTableToExcel(DataTable dt, string strFile, string sheetName = "Sheet0")
        {
            bool result = false;
            IWorkbook workbook = null;
            FileStream fs = null;
            IRow row = null;
            ISheet sheet = null;
            ICell cell = null;

            if (dt != null && dt.Rows.Count > 0)
            {
                workbook = new XSSFWorkbook();
                sheet = workbook.CreateSheet(sheetName);//创建一个名称为Sheet0的表
                int rowCount = dt.Rows.Count;//行数
                int columnCount = dt.Columns.Count;//列数

                //设置列头
                row = sheet.CreateRow(0);//excel第一行设为列头
                for (int c = 0; c < columnCount; c++)
                {
                    cell = row.CreateCell(c);
                    cell.SetCellValue(dt.Columns[c].ColumnName);
                }

                //设置每行每列的单元格,
                for (int i = 0; i < rowCount; i++)
                {
                    row = sheet.CreateRow(i + 1);
                    for (int j = 0; j < columnCount; j++)
                    {
                        cell = row.CreateCell(j);//excel第二行开始写入数据
                        cell.SetCellValue(dt.Rows[i][j].ToString());
                    }
                }
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    workbook.Write(memoryStream);//向打开的这个xls文件中写入数据
                    return memoryStream.ToArray();
                }
            }
            else
            {
                return new byte[0];
            }
        }

        /// <summary>
        /// List写入excel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="strFile">路径</param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
        public static bool ListWriteExcel<T>(T[] data, string strFile, string sheetName = "Sheet0")
        {
            bool result = false;
            IWorkbook workbook = null;
            FileStream fs = null;
            IRow row = null;
            ISheet sheet = null;
            ICell cell = null;
            try
            {
                var propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                       .OrderBy(p => p.GetCustomAttribute<ExeclPropertyAttribute>().Order).ToArray();
                workbook = new XSSFWorkbook();
                sheet = workbook.CreateSheet(sheetName);//创建一个名称为Sheet0的表
                int rowCount = data.Count();//行数
                int columnCount = propertyInfos.Length;//列数
                                                       //设置列头
                row = sheet.CreateRow(0);//excel第一行设为列头
                for (int c = 0; c < columnCount; c++)
                {
                    cell = row.CreateCell(c);
                    cell.SetCellValue(propertyInfos[c].GetCustomAttribute<ExeclPropertyAttribute>().DisplayName);
                }
                //设置每行每列的单元格,
                for (int i = 0; i < rowCount; i++)
                {
                    row = sheet.CreateRow(i + 1);
                    for (int j = 0; j < columnCount; j++)
                    {
                        cell = row.CreateCell(j);//excel第二行开始写入数据
                        cell.SetCellValue(propertyInfos[j].GetValue(data[i])?.ToString());
                    }
                }
                using (fs = File.OpenWrite(strFile))
                {
                    workbook.Write(fs);//向打开的这个xls文件中写入数据
                    result = true;
                }
                return result;
            }
            catch (Exception ex)
            {
                if (fs != null)
                {
                    fs.Close();
                }
                return false;
            }
        }

        /// <summary>
        /// dt写入excel
        /// </summary>
        /// <param name="dt">datatable</param>
        /// <param name="strFile">路径</param>
        /// <returns></returns>
        public static bool DataTableWriteExcel(DataTable dt, string strFile, string sheetName = "Sheet0")
        {
            bool result = false;
            IWorkbook workbook = null;
            FileStream fs = null;
            IRow row = null;
            ISheet sheet = null;
            ICell cell = null;
            try
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    workbook = new XSSFWorkbook();
                    sheet = workbook.CreateSheet(sheetName);//创建一个名称为Sheet0的表
                    int rowCount = dt.Rows.Count;//行数
                    int columnCount = dt.Columns.Count;//列数

                    //设置列头
                    row = sheet.CreateRow(0);//excel第一行设为列头
                    for (int c = 0; c < columnCount; c++)
                    {
                        cell = row.CreateCell(c);
                        cell.SetCellValue(dt.Columns[c].ColumnName);
                    }

                    //设置每行每列的单元格,
                    for (int i = 0; i < rowCount; i++)
                    {
                        row = sheet.CreateRow(i + 1);
                        for (int j = 0; j < columnCount; j++)
                        {
                            cell = row.CreateCell(j);//excel第二行开始写入数据
                            cell.SetCellValue(dt.Rows[i][j].ToString());
                        }
                    }
                    using (fs = File.OpenWrite(strFile))
                    {
                        workbook.Write(fs);//向打开的这个xls文件中写入数据
                        result = true;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                if (fs != null)
                {
                    fs.Close();
                }
                return false;
            }
        }

        /// <summary>
        /// 设置单元格下拉框(除去标题行)
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="ddlList"></param>
        /// <param name="firstcol"></param>
        /// <param name="lastcol"></param>
        public static void SetCellDropdownList(IWorkbook workbook, ISheet sheet, List<string> ddlList, string sheetname, int sheetIndex, int firstcol, int lastcol)
        {

            # region 低版本Excel【HSSFWorkbook】设置下拉框
            //ISheet sheet2 = workbook.CreateSheet(sheetname);

            ////隐藏
            //workbook.SetSheetHidden(sheetIndex, 1);
            //int rowIndex = 0;
            //foreach (var item in ddlList)
            //{
            //    IRow vrow = sheet2.CreateRow(rowIndex);
            //    vrow.CreateCell(0).SetCellValue(item);

            //    rowIndex++;
            //}

            ////创建的下拉项的区域：
            //var rangeName = sheetname + "Range";
            //IName range = workbook.CreateName();
            //range.RefersToFormula = sheetname + "!$A$1:$A$" + rowIndex;
            //range.NameName = rangeName;
            //CellRangeAddressList regions = new CellRangeAddressList(1, 65535, firstcol, lastcol);

            //DVConstraint constraint = DVConstraint.CreateFormulaListConstraint(rangeName);
            //HSSFDataValidation dataValidate = new HSSFDataValidation(regions, constraint);
            //dataValidate.CreateErrorBox("输入不合法", "请输入或选择下拉列表中的值。");
            //dataValidate.ShowPromptBox = true;
            //sheet.AddValidationData(dataValidate);
            #endregion

            //高版本excel【XSSFWorkbook】 设置下拉框
            XSSFSheet sheetDDL = (XSSFSheet)workbook.CreateSheet(sheetname);
            workbook.SetSheetHidden(sheetIndex, 1); //隐藏下拉框数据sheet
            String[] datas = ddlList.ToArray(); //下拉框数据源
            XSSFDataValidationHelper dvHelper = new XSSFDataValidationHelper(sheetDDL);
            XSSFDataValidationConstraint dvConstraint = (XSSFDataValidationConstraint)dvHelper.CreateExplicitListConstraint(datas);
            CellRangeAddressList addressList = new CellRangeAddressList(1, 65535, firstcol, lastcol); //下拉设置列
            XSSFDataValidation validation = (XSSFDataValidation)dvHelper.CreateValidation(dvConstraint, addressList);

            validation.SuppressDropDownArrow = true;
            validation.ShowErrorBox = true;
            validation.ShowPromptBox = true;
            sheet.AddValidationData(validation);

        }
    }
}
