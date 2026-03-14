// ============================================================
// ESG 企業永續數據盤查平台 - 全域 JavaScript
// ============================================================

// 自動關閉成功提示（3 秒後）
document.addEventListener('DOMContentLoaded', function () {
    const successAlerts = document.querySelectorAll('.alert-success.alert-dismissible');
    successAlerts.forEach(function (alert) {
        setTimeout(function () {
            const bsAlert = bootstrap.Alert.getOrCreateInstance(alert);
            bsAlert.close();
        }, 3000);
    });
});
