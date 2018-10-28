function parseJsonDate(jsonDate) {
    var offset = new Date().getTimezoneOffset() * 60000;
    var parts = /\/Date\((-?\d+)([+-]\d{2})?(\d{2})?.*/.exec(jsonDate);
    if (parts[2] == undefined) parts[2] = 0;
    if (parts[3] == undefined) parts[3] = 0;
    d = new Date(+parts[1] + offset + parts[2] * 3600000 + parts[3] * 60000);
    date = d.getDate() + 1;
    mon = d.getMonth() + 1;
    year = d.getFullYear();
    var values = CheckDate(date, mon, year);
    values[0] = values[0] < 10 ? "0" + values[0] : values[0];
    values[1] = values[1] < 10 ? "0" + values[1] : values[1];
    return (values[0] + "." + values[1] + "." + values[2]);
};

function CheckDate(day, month, year) {
    var returnValue = new Array();

    switch (month) {
        case 1: case 3: case 5: case 7: case 8: case 10: case 12: if (day > 31) {
            day = 1;
            if (month == 12) {
                year++;
                month = 1;
            }
            else {
                month++;
            }
        } break;
        case 4: case 6: case 9: case 11: if (day > 30) {
            day = 1;
            month++;
        } break;
        case 2: if ((day > 28 && year % 4 != 0) || (day > 29)) {
            day = 1;
            month++;
        } break;
        default:
    }

    returnValue[0] = day;
    returnValue[1] = month;
    returnValue[2] = year;

    return returnValue;
}