﻿using Dapper;
using MySqlConnector;
using QL.MUSIC.Common.Entities;
using QL.MUSIC.Common.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QL.MUSIC.DL
{
    public class BaseDL<T> : IBaseDL<T>
    {
        #region API Get
        /// <summary>
        /// Lấy danh sách toàn bộ bản ghi
        /// </summary>
        /// <returns>Danh sách toàn bộ bản ghi</returns>
        /// Cretaed by: NNNINH (10/11/2022)
        public IEnumerable<T> GetAllRecords()
        {
            // Khai báo tên stored procedure
            string storedProcedureName = String.Format(Resource.Proc_GetAll, typeof(T).Name);

            // Khởi tạo kết nối tới DB MySQL
            var records = new List<T>();
            using (var mysqlConnection = new MySqlConnection(DataContext.MySqlConnectionString))
            {
                // Thực hiện gọi vào DB
                records = (List<T>)mysqlConnection.Query<T>(
                   storedProcedureName,
                   commandType: System.Data.CommandType.StoredProcedure);
            }
            return records;
        }

        /// <summary>
        /// Lấy 1 bản ghi theo id
        /// </summary>
        /// <param name="recordId">ID của bản ghi cần lấy</param>
        /// <returns>Bản ghi có ID được truyền vào</returns>
        /// Created by:  NNNINH (10/11/2022)
        public T GetRecordById(Guid recordId)
        {
            // Chuẩn bị tham số đầu vào cho procedure
            var parameters = new DynamicParameters();
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var primaryKeyAttribute = (PrimaryKeyAttribute)Attribute.GetCustomAttribute(property, typeof(PrimaryKeyAttribute));
                if (primaryKeyAttribute != null)
                {
                    parameters.Add($"v_{property.Name}", recordId);
                    break;
                }
            }

            // Khởi tạo kết nối tới DB MySQL
            string connectionString = DataContext.MySqlConnectionString;
            T record;
            using (var mysqlConnection = new MySqlConnection(connectionString))
            {
                // Khai báo tên prodecure Insert
                string storedProcedureName = String.Format(Resource.Proc_Select, typeof(T).Name);

                // Thực hiện gọi vào DB để chạy procedure
                record = mysqlConnection.QueryFirstOrDefault<T>(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
            }
            return record;
        }
        #endregion

        #region API Insert
        /// <summary>
        /// Thêm mới 1 bản ghi
        /// </summary>
        /// <param name="record">Đối tượng bản ghi cần thêm mới</param>
        /// <returns>ID của bản ghi vừa thêm. Return về Guid rỗng nếu thêm mới thất bại</returns>
        /// Cretaed by: NNNINH (10/11/2022)
        public Guid InsertRecord(T record, Guid recordId)
        {
            // Chuẩn bị tham số đầu vào cho procedure
            var parameters = new DynamicParameters();
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                string propertyName = property.Name;
                object propertyValue;
                var primaryKeyAttribute = (PrimaryKeyAttribute)Attribute.GetCustomAttribute(property, typeof(PrimaryKeyAttribute));
                if (primaryKeyAttribute != null)
                {
                    propertyValue = recordId;
                }
                else
                {
                    propertyValue = property.GetValue(record, null);
                }
                parameters.Add($"v_{propertyName}", propertyValue);
            }

            // Khởi tạo kết nối tới DB MySQL
            string connectionString = DataContext.MySqlConnectionString;
            int numberOfAffectedRows = 0;
            using (var mysqlConnection = new MySqlConnection(connectionString))
            {
                // Khai báo tên prodecure Insert
                string storedProcedureName = String.Format(Resource.Proc_Add, typeof(T).Name);

                // Thực hiện gọi vào DB để chạy procedure
                numberOfAffectedRows = mysqlConnection.Execute(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
            }

            // Xử lý dữ liệu trả về
            if (numberOfAffectedRows > 0)
            {
                return recordId;
            }
            else
            {
                return Guid.Empty;
            }
        }
        #endregion


        #region API Update
        /// <summary>
        /// Cập nhật 1 bản ghi
        /// </summary>
        /// <param name="recordId">ID bản ghi cần cập nhật</param>
        /// <param name="record">Đối tượng cần cập nhật theo</param>
        /// <returns>ID của bản ghi sau khi cập nhật. Return về Guid rỗng nếu cập nhật thất bại</returns>
        /// Cretaed by: NNNINH (11/11/2022)
        public Guid UpdateRecord(Guid recordId, T record)
        {
            // Chuẩn bị tham số đầu vào cho procedure
            var parameters = new DynamicParameters();
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                string propertyName = property.Name;
                object propertyValue;
                var primaryKeyAttribute = (PrimaryKeyAttribute)Attribute.GetCustomAttribute(property, typeof(PrimaryKeyAttribute));
                if (primaryKeyAttribute != null)
                {
                    propertyValue = recordId;
                }
                else
                {
                    propertyValue = property.GetValue(record, null);
                }
                parameters.Add($"v_{propertyName}", propertyValue);
            }

            // Khởi tạo kết nối tới DB MySQL
            string connectionString = DataContext.MySqlConnectionString;
            int numberOfAffectedRows = 0;
            using (var mysqlConnection = new MySqlConnection(connectionString))
            {
                // Khai báo tên prodecure Insert
                string storedProcedureName = String.Format(Resource.Proc_Update, typeof(T).Name);

                // Thực hiện gọi vào DB để chạy procedure
                numberOfAffectedRows = mysqlConnection.Execute(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
            }
            // Xử lý dữ liệu trả về
            if (numberOfAffectedRows > 0)
            {
                return recordId;
            }
            else
            {
                return Guid.Empty;
            }
        }
        #endregion


        #region API Delete
        /// <summary>
        /// Xóa 1 bản ghi
        /// </summary>
        /// <param name="recordId">ID bản ghi cần xóa</param>
        /// <returns>ID bản ghi vừa xóa</returns>
        /// Cretaed by: NNNINH (11/11/2022)
        public Guid DeleteRecord(Guid recordId)
        {
            // Chuẩn bị tham số đầu vào cho procedure
            var parameters = new DynamicParameters();
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var primaryKeyAttribute = (PrimaryKeyAttribute)Attribute.GetCustomAttribute(property, typeof(PrimaryKeyAttribute));
                if (primaryKeyAttribute != null)
                {
                    parameters.Add($"v_{property.Name}", recordId);
                    break;
                }
            }

            // Khởi tạo kết nối tới DB MySQL
            string connectionString = DataContext.MySqlConnectionString;
            int numberOfAffectedRows = 0;
            using (var mysqlConnection = new MySqlConnection(connectionString))
            {
                // Khai báo tên prodecure Insert
                string storedProcedureName = String.Format(Resource.Proc_Delete, typeof(T).Name);

                // Thực hiện gọi vào DB để chạy procedure
                numberOfAffectedRows = mysqlConnection.Execute(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
            }
            // Xử lý dữ liệu trả về
            if (numberOfAffectedRows > 0)
            {
                return recordId;
            }
            else
            {
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Xóa nhiều bản ghi
        /// </summary>
        /// <param name="recordIdList">Danh sách ID các bản ghi cần xóa</param>
        /// <returns>Danh sách ID các bản ghi đã xóa</returns>
        /// Cretaed by:  NNNINH (11/11/2022)
        public int DeleteMultiRecords(List<string> recordIdList)
        {
            // Chuẩn bị tham số đầu vào cho procedure
            var properties = typeof(T).GetProperties();
            var propertyName = "";
            foreach (var property in properties)
            {
                var primaryKeyAttribute = (PrimaryKeyAttribute)Attribute.GetCustomAttribute(property, typeof(PrimaryKeyAttribute));
                if (primaryKeyAttribute != null)
                {
                    propertyName = property.Name;
                    break;
                }
            }

            // Khởi tạo kết nối tới DB MySQL
            string connectionString = DataContext.MySqlConnectionString;
            int numberOfAffectedRows = 0;
            using (var mysqlConnection = new MySqlConnection(connectionString))
            {

                // Khai báo tên prodecure 
                string storedProcedureName = String.Format(Resource.Proc_Delete, typeof(T).Name);

                //nếu như kết nối đang đóng thì tiến hành mở lại
                if (mysqlConnection.State != ConnectionState.Open)
                {
                    mysqlConnection.Open();
                }

                // Bắt đầu transaction.
                using (var transaction = mysqlConnection.BeginTransaction())
                {
                    try
                    {
                        for (int i = 0; i < recordIdList.Count; i++)
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add($"v_{propertyName}", recordIdList[i]);
                            numberOfAffectedRows += mysqlConnection.Execute(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure, transaction: transaction);
                        }

                        if (numberOfAffectedRows == recordIdList.Count)
                        {
                            transaction.Commit();

                        }
                        else
                        {
                            transaction.Rollback();
                            numberOfAffectedRows = 0;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        //nếu thực hiện không thành công thì rollback
                        transaction.Rollback();
                        numberOfAffectedRows = 0;
                    }
                    finally
                    {
                        mysqlConnection.Close();
                    }

                }
            }
            return numberOfAffectedRows;
        }
        #endregion

        #region API DuplicateCode
        /// <summary>
        /// Kiểm tra trùng mã bản ghi
        /// </summary>
        /// <param name="recordCode">Mã cần xét trùng</param>
        /// <param name="recordId">Id bản ghi đưa vào (nếu là sửa)</param>
        /// <returns>Số lượng mã tài sản bị trùng</returns>
        /// Cretaed by: NNNINH (10/11/2022)
        public int DuplicateRecordCode(object recordCode, Guid recordId)
        {
            // Khai báo tên prodecure
            string storedProcedureName = String.Format(Resource.Proc_DuplicateCode, typeof(T).Name);

            // Chuẩn bị tham số đầu vào cho procedure
            var parameters = new DynamicParameters();
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                var isNotDuplicateAttribute = (IsNotDuplicateAttribute)Attribute.GetCustomAttribute(property, typeof(IsNotDuplicateAttribute));
                if (isNotDuplicateAttribute != null)
                {
                    parameters.Add($"v_{property.Name}", recordCode);
                }

                var primaryKeyAttribute = (PrimaryKeyAttribute)Attribute.GetCustomAttribute(property, typeof(PrimaryKeyAttribute));
                if (primaryKeyAttribute != null)
                {
                    parameters.Add($"v_{property.Name}", recordId);
                }
            }

            // Khởi tạo kết nối tới DB MySQL
            string connectionString = DataContext.MySqlConnectionString;
            int duplicates = 0;
            using (var mysqlConnection = new MySqlConnection(connectionString))
            {
                duplicates = mysqlConnection.QueryFirstOrDefault<int>(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
            }

            return duplicates;
        }
        #endregion

        public PagingData<T> FilterRecords(string? keyword, string type)
        {
            throw new NotImplementedException();
        }

        PagingData<T> IBaseDL<T>.FilterRecords(string? keyword, string type)
        {
            throw new NotImplementedException();
        }
    }
}
