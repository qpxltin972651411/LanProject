using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanProject.MainApplication.Map
{
    public enum CorrectCodeList
    {
        [Description("[自單位] - 新增單位成功")]
        CORRECT_SELFCOMPANY_PAGE_CREATE_UNIT_SUCCESS = 0x00008C00,
        [Description("[自單位] - 刪除單位成功")]
        CORRECT_SELFCOMPANY_PAGE_DELETE_UNIT_SUCCESS = 0x00008C01,
        [Description("[自單位] - 編輯單位成功")]
        CORRECT_SELFCOMPANY_PAGE_EDIT_UNIT_SUCCESS = 0x00008C02,

        [Description("[他單位] - 新增單位成功")]
        CORRECT_CUSTOMCOMPANY_PAGE_CREATE_UNIT_SUCCESS = 0x00009C00,
        [Description("[他單位] - 刪除單位成功")]
        CORRECT_CUSTOMCOMPANY_PAGE_DELETE_UNIT_SUCCESS = 0x00009C01,
        [Description("[他單位] - 編輯單位成功")]
        CORRECT_CUSTOMCOMPANY_PAGE_EDIT_UNIT_SUCCESS = 0x00009C02,

        [Description("[表單] - 新增表單成功")]
        CORRECT_FORM_PAGE_CREATE_FORM_SUCCESS = 0x0000AC00,
        [Description("[表單] - 刪除表單成功")]
        CORRECT_FORM_PAGE_DELETE_FORM_SUCCESS = 0x0000AC01,
        [Description("[表單] - 編輯表單成功")]
        CORRECT_FORM_PAGE_EDIT_FORM_SUCCESS = 0x0000AC02,

        [Description("[備註設定] - 新增備註成功")]
        CORRECT_SETTING_PAGE_CREATE_NOTE_SUCCESS = 0x0000BC00,
        [Description("[備註設定] - 修改備註成功")]
        CORRECT_SETTING_PAGE_EDIT_NOTE_SUCCESS = 0x0000BC01,
    }
    public enum ErrorCodeList
    {
        #region 自單位
        [Description("[自單位] - 已達頁尾")]
        ERROR_SELFCOMPANY_PAGE_NEXT_PAGE_FAILED = 0x00006D00,

        [Description("[自單位] - 無法載入單位 未知錯誤")]
        ERROR_SELFCOMPANY_PAGE_LOAD_UNIT_UNKNOWN_FAILED = 0x00007D00,

        [Description("[自單位] - 無法新增單位 欄位格式有誤")]
        ERROR_SELFCOMPANY_PAGE_CREATE_UNIT_VERIFY_INPUT_FAILED = 0x00008D00,
        [Description("[自單位] - 無法新增單位 未知錯誤")]
        ERROR_SELFCOMPANY_PAGE_CREATE_UNIT_UNKNOWN_FAILED = 0x00008D01,
        [Description("[自單位] - 無法新增單位 已有重複單位")]
        ERROR_SELFCOMPANY_PAGE_CREATE_UNIT_REPEAT_FAILED = 0x00008D02,
        [Description("[自單位] - 無法新增單位 寫入發生錯誤")]
        ERROR_SELFCOMPANY_PAGE_CREATE_UNIT_WRITING_FAILED = 0x00008D03,

        [Description("[自單位] - 無法刪除單位 未知錯誤")]
        ERROR_SELFCOMPANY_PAGE_DELETE_UNIT_UNKNOWN_FAILED = 0x00009D00,
        [Description("[自單位] - 無法刪除單位 刪除發生錯誤")]
        ERROR_SELFCOMPANY_PAGE_DELETE_UNIT_WRITING_FAILED = 0x00009D01,

        [Description("[自單位] - 無法編輯單位 欄位格式有誤")]
        ERROR_SELFCOMPANY_PAGE_EDIT_UNIT_VERIFY_INPUT_FAILED = 0x0000AD00,
        [Description("[自單位] - 無法編輯單位 未知錯誤")]
        ERROR_SELFCOMPANY_PAGE_EDIT_UNIT_UNKNOWN_FAILED = 0x0000AD01,
        [Description("[自單位] - 無法編輯單位 已有重複單位")]
        ERROR_SELFCOMPANY_PAGE_EDIT_UNIT_REPEAT_FAILED = 0x0000AD02,
        [Description("[自單位] - 無法編輯單位 寫入發生錯誤")]
        ERROR_SELFCOMPANY_PAGE_EDIT_UNIT_WRITING_FAILED = 0x0000AD03,
        #endregion
        #region 他單位
        [Description("[他單位] - 已達頁尾")]
        ERROR_CUSTOMCOMPANY_PAGE_NEXT_PAGE_FAILED = 0x00016D00,

        [Description("[他單位] - 無法載入單位 未知錯誤")]
        ERROR_CUSTOMCOMPANY_PAGE_LOAD_UNIT_UNKNOWN_FAILED = 0x00017D00,

        [Description("[他單位] - 無法新增單位 欄位格式有誤")]
        ERROR_CUSTOMCOMPANY_PAGE_CREATE_UNIT_VERIFY_INPUT_FAILED = 0x00018D00,
        [Description("[他單位] - 無法新增單位 未知錯誤")]
        ERROR_CUSTOMCOMPANY_PAGE_CREATE_UNIT_UNKNOWN_FAILED = 0x00018D01,
        [Description("[他單位] - 無法新增單位 已有重複單位")]
        ERROR_CUSTOMCOMPANY_PAGE_CREATE_UNIT_REPEAT_FAILED = 0x00018D02,
        [Description("[他單位] - 無法新增單位 寫入發生錯誤")]
        ERROR_CUSTOMCOMPANY_PAGE_CREATE_UNIT_WRITING_FAILED = 0x00018D03,

        [Description("[他單位] - 無法刪除單位 未知錯誤")]
        ERROR_CUSTOMCOMPANY_PAGE_DELETE_UNIT_UNKNOWN_FAILED = 0x00019D00,
        [Description("[他單位] - 無法刪除單位 刪除發生錯誤")]
        ERROR_CUSTOMCOMPANY_PAGE_DELETE_UNIT_WRITING_FAILED = 0x00019D01,

        [Description("[他單位] - 無法編輯單位 欄位格式有誤")]
        ERROR_CUSTOMCOMPANY_PAGE_EDIT_UNIT_VERIFY_INPUT_FAILED = 0x0001AD00,
        [Description("[他單位] - 無法編輯單位 未知錯誤")]
        ERROR_CUSTOMCOMPANY_PAGE_EDIT_UNIT_UNKNOWN_FAILED = 0x0001AD01,
        [Description("[他單位] - 無法編輯單位 已有重複單位")]
        ERROR_CUSTOMCOMPANY_PAGE_EDIT_UNIT_REPEAT_FAILED = 0x0001AD02,
        [Description("[他單位] - 無法編輯單位 寫入發生錯誤")]
        ERROR_CUSTOMCOMPANY_PAGE_EDIT_UNIT_WRITING_FAILED = 0x0001AD03,
        #endregion
        #region 表單
        [Description("[表單] - 已達頁尾")]
        ERROR_FORM_PAGE_NEXT_PAGE_FAILED = 0x00026D00,

        [Description("[表單] - 無法載入表單 未知錯誤")]
        ERROR_FORM_PAGE_LOAD_UNIT_UNKNOWN_FAILED = 0x00027D00,

        [Description("[表單] - 無法新增表單 欄位格式有誤")]
        ERROR_FORM_PAGE_CREATE_UNIT_VERIFY_INPUT_FAILED = 0x00028D00,
        [Description("[表單] - 無法新增表單 未知錯誤")]
        ERROR_FORM_PAGE_CREATE_UNIT_UNKNOWN_FAILED = 0x00028D01,
        [Description("[表單] - 無法新增表單 已有重複單位")]
        ERROR_FORM_PAGE_CREATE_UNIT_REPEAT_FAILED = 0x00028D02,
        [Description("[表單] - 無法新增表單 寫入發生錯誤")]
        ERROR_FORM_PAGE_CREATE_UNIT_WRITING_FAILED = 0x00028D03,

        [Description("[表單] - 無法刪除表單 未知錯誤")]
        ERROR_FORM_PAGE_DELETE_UNIT_UNKNOWN_FAILED = 0x00029D00,
        [Description("[表單] - 無法刪除表單 刪除發生錯誤")]
        ERROR_FORM_PAGE_DELETE_UNIT_WRITING_FAILED = 0x00029D01,

        [Description("[表單] - 無法編輯表單 欄位格式有誤")]
        ERROR_FORM_PAGE_EDIT_UNIT_VERIFY_INPUT_FAILED = 0x0002AD00,
        [Description("[表單] - 無法編輯表單 未知錯誤")]
        ERROR_FORM_PAGE_EDIT_UNIT_UNKNOWN_FAILED = 0x0002AD01,
        [Description("[表單] - 無法編輯表單 已有重複單位")]
        ERROR_FORM_PAGE_EDIT_UNIT_REPEAT_FAILED = 0x0002AD02,
        [Description("[表單] - 無法編輯表單 寫入發生錯誤")]
        ERROR_FORM_PAGE_EDIT_UNIT_WRITING_FAILED = 0x0002AD03,
        #endregion
        #region 設定
        [Description("[備註設定] - 有重複名稱 請重新命名")]
        ERROR_SETTING_PAGE_CREATE_NOTE_FAILED_NAME_REPEAT = 0x00036D00,
        [Description("[備註設定] - 文件命名失敗 請嘗試重新命名")]
        ERROR_SETTING_PAGE_CREATE_NOTE_FAILED_NAME_ILLEGAL = 0x00036D01,
        [Description("[備註設定] - 文件命名失敗 名稱相同未異動")]
        ERROR_SETTING_PAGE_CREATE_NOTE_FAILED_NAME_NOT_CHANGE = 0x00036D02,
        #endregion
    }
}
