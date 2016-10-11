using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using System.Data;
using System.IO;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Reflection;
using System.ComponentModel;
using NPOI.SS.Util;
using NPOI;
using NPOI.XSSF.UserModel;
using FRXS.Common.IO;


namespace FRXS.Common.Excel
{
    /// <summary>
    /// Npoi操作Excel帮助类
    /// </summary>
    public class NpoiExcelhelper
    {
        #region 公有方法

        #region 集合导出到Excel文件
        /// <summary>
        /// 集合导出到Excel文件
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <param name="list">集合</param>
        /// <param name="maxRows">单个sheet行数限制</param>
        /// <param name="directoryPath">服务端保存目录</param>
        /// <param name="fileName">文件名</param>
        /// <returns>字节数组</returns>
        public static byte[] ExportExcel<T>(IList<T> list, int maxRows, string directoryPath, string fileName) where T : class,new()
        {
            MemoryStream ms = RenderToExcel(list, maxRows);
            byte[] data = SaveFile(ms, directoryPath, fileName);
            return data;
        }
        #endregion

        #region DataTable导出到Excel文件
        /// <summary>
        /// DataTable导出到Excel文件
        /// </summary>
        /// <param name="table">数据源</param>
        /// <param name="maxRows">单个sheet行数限制</param>
        /// <param name="directoryPath">服务端保存目录</param>
        /// <param name="fileName">文件名</param>
        /// <returns>字节数组</returns>
        public static byte[] ExportExcel(DataTable table, int maxRows, string directoryPath, string fileName)
        {
            MemoryStream ms = RenderToExcel(table, maxRows);
            byte[] data = SaveFile(ms, directoryPath, fileName);
            return data;
        }
        #endregion

        #region DataSet导出到Excel文件
        /// <summary>
        /// DataSet导出到Excel文件
        /// </summary>
        /// <param name="dataset">数据源</param>
        /// <param name="maxRows">单个sheet行数限制</param>
        /// <param name="directoryPath">服务端保存目录</param>
        /// <param name="fileName">文件名</param>
        /// <returns>字节数组</returns>
        public static byte[] ExportExcel(DataSet dataset, int maxRows, string directoryPath, string fileName)
        {
            MemoryStream ms = RenderToExcel(dataset, maxRows);
            byte[] data = SaveFile(ms, directoryPath, fileName);
            return data;
        }
        #endregion

        #region 从url读取Excel文件至DataTable
        /// <summary>
        /// 从url读取Excel文件至DataTable
        /// </summary>
        /// <param name="url">Excel文件的服务端Url</param>
        /// <returns>DataTable</returns>
        public static DataTable RenderFromExcel(string url)
        {
            Stream s = FileHandle.ReadFileByUrl(url);
            return RenderFromExcel(s);
        }
        #endregion

        #region 从Excel文件流读取文件至DataTable
        /// <summary>
        /// 从Excel文件流读取文件至DataTable
        /// </summary>
        /// <param name="excelFileStream">Excel文件流</param>
        /// <param name="isDeleteFile">是否删除服务器上文件</param>
        /// <returns>DataTable</returns>
        public static DataTable RenderFromExcel(Stream excelFileStream)
        {
            using (excelFileStream)
            {
                IWorkbook workbook = GetWorkbookByStream(excelFileStream);

                ISheet sheet = workbook.GetSheetAt(0);//取第一个表

                DataTable table = new DataTable();

                IRow headerRow = sheet.GetRow(0);//第一行为标题行
                int cellCount = headerRow.LastCellNum;//LastCellNum = PhysicalNumberOfCells
                int rowCount = sheet.LastRowNum;//LastRowNum = PhysicalNumberOfRows - 1

                //handling header.
                for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                {
                    DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                    table.Columns.Add(column);
                }

                for (int i = (sheet.FirstRowNum + 1); i <= rowCount; i++)
                {
                    IRow row = sheet.GetRow(i);
                    DataRow dataRow = table.NewRow();

                    if (row != null)
                    {
                        for (int j = row.FirstCellNum; j < cellCount; j++)
                        {
                            if (row.GetCell(j) != null)
                                dataRow[j] = GetCellValue(row.GetCell(j));
                        }
                    }

                    table.Rows.Add(dataRow);
                }
                excelFileStream.Close();
                excelFileStream.Dispose();
                return table;
            }
        }
        #endregion

        #endregion

        #region 受保护方法

        #region 根据Excel列类型获取列值
        /// <summary>
        /// 根据Excel列类型获取列值
        /// </summary>
        /// <param name="cell">Excel列</param>
        /// <returns>值</returns>
        protected static string GetCellValue(ICell cell)
        {
            if (cell == null)
            {
                return string.Empty;
            }
            switch (cell.CellType)
            {
                case CellType.Blank:
                    return string.Empty;
                case CellType.Boolean:
                    return cell.BooleanCellValue.ToString();
                case CellType.Error:
                    return cell.ErrorCellValue.ToString();
                case CellType.String:
                    return cell.StringCellValue;
                case CellType.Formula:
                    try
                    {
                        HSSFFormulaEvaluator e = new HSSFFormulaEvaluator(cell.Sheet.Workbook);
                        e.EvaluateInCell(cell);
                        return cell.ToString();
                    }
                    catch
                    {
                        return cell.NumericCellValue.ToString();
                    }
                case CellType.Numeric:
                    return cell.NumericCellValue.ToString();
                case CellType.Unknown:
                default:
                    return cell.ToString();//This is a trick to get the correct value of the cell. NumericCellValue will return a numeric value no matter the cell value is a date or a number
            }
        }
        #endregion

        #region 自动设置Excel列宽
        /// <summary>
        /// 自动设置Excel列宽
        /// </summary>
        /// <param name="sheet">Excel表</param>
        protected static void AutoSizeColumns(ISheet sheet)
        {
            if (sheet.PhysicalNumberOfRows > 0)
            {
                IRow headerRow = sheet.GetRow(0);

                for (int i = 0, l = headerRow.LastCellNum; i < l; i++)
                {
                    sheet.AutoSizeColumn(i);
                }
            }
        }
        #endregion

        #region 检查流中是否存在数据
        /// <summary>
        /// 检查流中是否存在数据
        /// </summary>
        /// <param name="excelFileStream">Excel文件流</param>
        /// <returns>是否存在数据</returns>
        public static bool HasData(Stream excelFileStream)
        {
            using (excelFileStream)
            {
                IWorkbook workbook = GetWorkbookByStream(excelFileStream);
                if (workbook.NumberOfSheets > 0)
                {
                    ISheet sheet = workbook.GetSheetAt(0);
                    return sheet.PhysicalNumberOfRows > 0;
                }
            }
            return false;
        }
        #endregion

        #region DataTable读取至内存
        /// <summary>
        /// DataTable读取至内存
        /// </summary>
        /// <param name="dataset">DataSet</param>
        /// <param name="maxRows">单个sheet最大限制行数</param>
        /// <param name="groupbyFields">分组字段集合</param>
        /// <param name="mergeFields">合并字段集合</param>
        /// <returns>内存流</returns>
        protected static MemoryStream RenderToExcel(DataSet dataset, int maxRows, List<string> groupbyFields = null, List<string> mergeFields = null)
        {
            MemoryStream ms = new MemoryStream();
            IWorkbook workbook = GetWorkbookByStream(null);

            foreach (DataTable table in dataset.Tables)
            {
                RenderToExcelByDataTable(ms, workbook, table, maxRows, groupbyFields, mergeFields);
            }

            workbook.Write(ms);
            ms.Flush();
            ms.Position = 0;
            return ms;
        }
        #endregion

        #region DataTable读取至内存
        /// <summary>
        /// DataTable读取至内存
        /// </summary>
        /// <param name="table">DataTable</param>
        /// <param name="maxRows">单个sheet最大限制行数</param>
        /// <param name="groupbyFields">分组字段集合</param>
        /// <param name="mergeFields">合并字段集合</param>
        /// <returns>内存流</returns>
        protected static MemoryStream RenderToExcel(DataTable table, int maxRows, List<string> groupbyFields = null, List<string> mergeFields = null)
        {
            MemoryStream ms = new MemoryStream();
            IWorkbook workbook = GetWorkbookByStream(null);

            RenderToExcelByDataTable(ms, workbook, table, maxRows, groupbyFields, mergeFields);

            workbook.Write(ms);
            ms.Flush();
            ms.Position = 0;
            return ms;
        }
        #endregion

        #region 从DataTable读取Excel对象至内存
        /// <summary>
        /// 从DataTable读取Excel对象至内存
        /// </summary>
        /// <param name="ms">内存流</param>
        /// <param name="workbook">Excel对象</param>
        /// <param name="table">DataTable对象</param>
        /// <param name="maxRows">单个sheet最大行数</param>
        /// <param name="groupbyFields">分组字段集合</param>
        /// <param name="mergeFields">合并字段集合</param>
        protected static void RenderToExcelByDataTable(MemoryStream ms, IWorkbook workbook, DataTable table, int maxRows, List<string> groupbyFields = null, List<string> mergeFields = null)
        {
            ISheet sheet = null;

            int rowIndex = 1;
            int sheetNo = 1;
            foreach (DataRow row in table.Rows)
            {
                //如果为第一行数据，或者已填充了指定限制行数数据，则新创建sheet
                if (rowIndex == 1 || (maxRows > 0 && (rowIndex - 1) % maxRows == 0))
                {
                    //如果table名称不为null，则使用table名称加序号作为sheet名称，否则则使用sheet加序号作为sheet名称
                    sheet = workbook.CreateSheet((table.TableName ?? "sheet") + sheetNo);
                    IRow headerRow = sheet.CreateRow(0);
                    foreach (DataColumn column in table.Columns)
                    {
                        headerRow.CreateCell(column.Ordinal).SetCellValue(column.Caption);
                    }
                    sheetNo++;
                    rowIndex = 1;
                }
                IRow dataRow = sheet.CreateRow(rowIndex);

                foreach (DataColumn column in table.Columns)
                {
                    dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
                }

                rowIndex++;
            }

            //合并单元格
            MergeCells(workbook, null, groupbyFields, mergeFields);
        }
        #endregion

        #region 从内存输出文件流至字节数组
        /// <summary>
        /// 从内存输出文件流至字节数组
        /// </summary>
        /// <param name="memoryStream">内存流</param>
        /// <param name="directoryPath">服务端保存目录</param>
        /// <param name="fileName">文件名</param>
        /// <returns>字节数组</returns>
        protected static byte[] SaveFile(MemoryStream memoryStream, string directoryPath, string fileName)
        {
            byte[] data = null;

            fileName = CreateFilePath(directoryPath, fileName);

            using (MemoryStream ms = memoryStream)
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
            }
            //删除服务器上的文件
            File.Delete(fileName);
            return data;
        }
        #endregion

        #region 创建服务端文件路径
        /// <summary>
        /// 创建服务端文件路径
        /// </summary>
        /// <param name="directoryPath">服务端保存目录</param>
        /// <param name="fileName">文件名</param>
        /// <returns>文件路径</returns>
        protected static string CreateFilePath(string directoryPath, string fileName)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            fileName = directoryPath + "\\" + fileName;
            return fileName;
        }
        #endregion

        #region 集合读取至内存
        /// <summary>
        /// 集合读取至内存
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <param name="list">集合</param>
        /// <param name="maxRows">单个sheet最大限制行数</param>
        /// <returns>内存流</returns>
        protected static MemoryStream RenderToExcel<T>(IList<T> list, int maxRows) where T : class,new()
        {
            MemoryStream ms = new MemoryStream();

            IWorkbook workbook = GetWorkbookByStream(null);
            ISheet sheet = null;

            //通过反射获取传入的集合的对象的所有公有属性，存入属性数组
            PropertyInfo[] propertyInfo = typeof(T).GetProperties();

            int rowIndex = 1;
            int sheetNo = 1;
            for (int i = 0; i < list.Count; i++)
            {
                //如果为第一行数据，或者已填充了指定限制行数数据，则新创建sheet
                if (rowIndex == 1 || (maxRows > 0 && (rowIndex - 1) % maxRows == 0))
                {
                    //如果table名称不为null，则使用table名称加序号作为sheet名称，否则则使用sheet加序号作为sheet名称
                    sheet = workbook.CreateSheet((list[i].GetType().Name ?? "sheet") + sheetNo);
                    IRow headerRow = sheet.CreateRow(0);

                    for (int j = 0, k = 0; j < propertyInfo.Length; j++)
                    {
                        //bool isExportField = GetExportFields(propertyInfo[j]);
                        //if (isExportField)
                        //{
                            headerRow.CreateCell(k).SetCellValue(GetDisplayAttributeByPropertyName(propertyInfo, propertyInfo[j].Name));
                            k++;
                        //}
                    }
                    sheetNo++;
                    rowIndex = 1;
                }
                IRow dataRow = sheet.CreateRow(rowIndex);

                for (int j = 0, k = 0; j < propertyInfo.Length; j++)
                {
                    //bool isExportField = GetExportFields(propertyInfo[j]);
                    //if (isExportField)
                    //{
                        object value = propertyInfo[j].GetValue(list[i], null);
                        dataRow.CreateCell(k).SetCellValue(value != null ? value.ToString() : "");
                        k++;
                    //}
                }

                rowIndex++;
            }

            //合并单元格
            MergeCells(workbook, propertyInfo);

            workbook.Write(ms);
            ms.Flush();
            ms.Position = 0;
            return ms;
        }
        #endregion

        #region 获取导出字段
        /// <summary>
        /// 获取导出字段
        /// </summary>
        /// <param name="propertyInfo">字段</param>
        /// <returns>是否属于导出字段</returns>
        //protected static bool GetExportFields(PropertyInfo propertyInfo)
        //{
        //    bool isExportField = !(propertyInfo.CustomAttributes.Where(p => p.AttributeType == typeof(ExcelNoExportAttribute)).Count() > 0);
        //    return isExportField;
        //}
        #endregion

        #region 通过属性名称获取该属性对象客户端显示文本特性字符串
        /// <summary>
        /// 通过属性名称获取该属性对象客户端显示文本特性字符串
        /// </summary>
        /// <param name="propertyInfoArray">属性数组</param>
        /// <param name="propertyInfoName">属性名称</param>
        /// <returns>客户端显示文本特性字符串</returns>
        protected static string GetDisplayAttributeByPropertyName(PropertyInfo[] propertyInfoArray, string propertyInfoName)
        {
            foreach (PropertyInfo pi in propertyInfoArray)
            {
                if (pi.Name == propertyInfoName)
                {
                    //获取属性对象客户端特性数组
                    object[] customAttributes = pi.GetCustomAttributes(false);
                    foreach (object customAttribute in customAttributes)
                    {
                        //获取属性对象显示文本特性对象
                        DisplayNameAttribute displayNameAttribute = customAttribute is DisplayNameAttribute ? (DisplayNameAttribute)customAttribute : null;
                        if (displayNameAttribute != null)
                        {
                            //赋值属性对象显示文本特性文本值
                            return displayNameAttribute.DisplayName;
                        }
                    }
                }
            }
            return string.Empty;
        }
        #endregion

        #region 合并单元格
        /// <summary>
        /// 合并单元格
        /// </summary>
        /// <param name="workbook">Excel工作簿对象</param>
        /// <param name="propertyInfo">属性数组</param>
        /// <param name="groupbyFields">分组字段集合</param>
        /// <param name="mergeFields">合并字段集合</param>
        protected static void MergeCells(IWorkbook workbook, PropertyInfo[] propertyInfo = null, List<string> groupbyFields = null, List<string> mergeFields = null)
        {
            if (propertyInfo != null)
            {
                //分组字段集合
                groupbyFields = new List<string>();
                //合并字段集合
                mergeFields = new List<string>();

                //根据特性标识获取Excel分组及合并字段集合
                //GetExcelMergeAttributeList(propertyInfo, groupbyFields, mergeFields);
            }

            if (groupbyFields != null && groupbyFields.Count > 0 && mergeFields != null && mergeFields.Count > 0)
            {
                //定义分组字段值集合
                List<string> groupbyFieldsValues = new List<string>();
                //定义合并字段值集合
                List<string> mergeFieldsValues = new List<string>();
                //合并开始行标记
                int rowMergeStartFlag = 0;
                //合并结束行标记
                int rowMergeEndFlag = 0;
                for (int count = 0; count < workbook.NumberOfSheets; count++)
                {
                    ISheet sheet = workbook.GetSheetAt(count);
                    //合并单元格
                    for (int i = 1; i <= sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);

                        //是否属于需合并数据
                        bool isHasMergeRow = false;
                        if (groupbyFieldsValues.Count > 0)
                        {
                            //取出该行中所有单元格中的值
                            List<string> cellValues = row.Cells.Select(p => p.StringCellValue).ToList();
                            List<string> cross = cellValues.Intersect(groupbyFieldsValues).ToList<string>();
                            if (cross != null)
                            {
                                isHasMergeRow = (cross.Count == groupbyFieldsValues.Count);
                            }
                        }

                        for (int j = 0; j < row.LastCellNum; j++)
                        {
                            //判断sheet中该字段是否属于分组字段
                            bool isExists = groupbyFields.Contains(propertyInfo[j].Name);
                            if (isExists)
                            {
                                //如果分组字段值集合有值
                                if (groupbyFieldsValues.Count > 0)
                                {
                                    //如果符合合并条件
                                    if (isHasMergeRow)
                                    {
                                        rowMergeEndFlag = i;

                                        //如果符合以下条件，则合并分组字段
                                        if (i == sheet.LastRowNum && rowMergeStartFlag > 0 && rowMergeEndFlag > 0 && rowMergeEndFlag > rowMergeStartFlag)
                                        {
                                            sheet.AddMergedRegion(new CellRangeAddress(rowMergeStartFlag, rowMergeEndFlag, j, j));
                                        }
                                    }
                                    else
                                    {
                                        //如果符合以下条件，则合并分组字段
                                        if (rowMergeStartFlag > 0 && rowMergeEndFlag > 0 && rowMergeEndFlag > rowMergeStartFlag)
                                        {
                                            sheet.AddMergedRegion(new CellRangeAddress(rowMergeStartFlag, rowMergeEndFlag, j, j));
                                        }

                                        groupbyFieldsValues.Clear();

                                        rowMergeStartFlag = i;
                                        groupbyFieldsValues.Add(row.GetCell(j).StringCellValue);
                                    }
                                }
                                else
                                {
                                    rowMergeStartFlag = i;
                                    groupbyFieldsValues.Add(row.GetCell(j).StringCellValue);
                                }
                            }
                            else
                            {
                                //如果符合以下条件，则存在需要合并的行，并且当前行为合并行的下一行数据
                                if (rowMergeStartFlag > 0 && rowMergeEndFlag > 0 && rowMergeEndFlag > rowMergeStartFlag)
                                {
                                    isExists = mergeFields.Contains(propertyInfo[j].Name);
                                    if (isExists)
                                    {
                                        sheet.AddMergedRegion(new CellRangeAddress(rowMergeStartFlag, rowMergeEndFlag, j, j));
                                    }
                                }
                            }
                        }
                    }

                    //重置分组字段值集合
                    groupbyFieldsValues.Clear();
                    //重置合并字段值集合
                    mergeFieldsValues.Clear();
                    //重置合并开始行标记
                    rowMergeStartFlag = 0;
                    //重置合并结束行标记
                    rowMergeEndFlag = 0;
                }
            }
        }
        #endregion

        #region 根据特性标识获取Excel分组及合并字段集合
        /// <summary>
        /// 根据特性标识获取Excel分组及合并字段集合
        /// </summary>
        /// <param name="propertyInfoArray">属性数组</param>
        /// <param name="groupbyFields">分组字段集合</param>
        /// <param name="mergeFields">合并字段集合</param>
        //protected static void GetExcelMergeAttributeList(PropertyInfo[] propertyInfoArray, List<string> groupbyFields, List<string> mergeFields)
        //{
        //    foreach (PropertyInfo pi in propertyInfoArray)
        //    {
        //        //获取属性对象客户端特性数组
        //        object[] customAttributes = pi.GetCustomAttributes(false);
        //        foreach (object customAttribute in customAttributes)
        //        {
        //            ExcelGroupByAttribute groupbyAttribute = customAttribute is ExcelGroupByAttribute ? (ExcelGroupByAttribute)customAttribute : null;
        //            if (groupbyAttribute != null)
        //            {
        //                if (groupbyFields == null)
        //                {
        //                    groupbyFields = new List<string>();
        //                }
        //                groupbyFields.Add(pi.Name);
        //            }

        //            ExcelMergeAttribute mergeAttribute = customAttribute is ExcelMergeAttribute ? (ExcelMergeAttribute)customAttribute : null;
        //            if (mergeAttribute != null)
        //            {
        //                if (mergeAttribute == null)
        //                {
        //                    mergeFields = new List<string>();
        //                }
        //                mergeFields.Add(pi.Name);
        //            }
        //        }
        //    }
        //}
        #endregion

        #region 根据excel文件流创建workbook对象（自动识别2003和2007版本）
        /// <summary>
        /// 根据excel文件流创建workbook对象（自动识别2003和2007版本）
        /// </summary>
        /// <param name="excelFileStream">excel文件流</param>
        /// <returns>workbook对象</returns>
        protected static IWorkbook GetWorkbookByStream(Stream excelFileStream)
        {
            IWorkbook workbook = null;
            if (excelFileStream == null)
            {
                workbook = new HSSFWorkbook();
            }
            else
            {
                if (POIXMLDocument.HasOOXMLHeader(excelFileStream))//2007版本
                {
                    try
                    {
                        workbook = new XSSFWorkbook(excelFileStream);
                    }
                    catch
                    {
                        throw new Exception("请确认文件是否为Excel2007格式");
                    }
                }
                else//2003版本
                {
                    try
                    {
                        workbook = new HSSFWorkbook(excelFileStream);
                    }
                    catch
                    {
                        throw new Exception("请确认文件是否为Excel2003格式");
                    }
                }
            }
            return workbook;
        }
        #endregion

        #endregion
    }
}
