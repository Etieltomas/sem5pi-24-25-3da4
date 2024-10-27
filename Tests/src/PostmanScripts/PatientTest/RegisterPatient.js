/*
Cookie: .AspNetCore.Cookies=CfDJ8NqQwVH9wspFt2nkQ4EBMjMbSsR7Wi4LujLDEGWzDQCAK0Yj5xpaixm-2FpgrhaxSZAXSrTfAxFv9mZzWiraVBcGreAZ--DW5sJQ7Lt0MBn7A2OLuje7h-KLT1ONxBHwOC8S1ocarDr8FzJ-srbsmgsfirK3zKfEOWvjPippgw4q1ltx7DLEJMhCC1Ijt3IPgtQj-h8zcWzXTZwYob56Yw6oSDhpVDniX46ja8Nd_KRbcYyffBxrTSIoTtBbVEJMWkOwAM-_itPa9Ah_wkgQHNWS6iHNZY8-GEqnsNLUQNCdW_CAdNVqsAiaClA_O44-WBIOhXfajcKPhiO9lry8C-25efse_Uzf8fTPgZZR-doDKmgo2djXuxcMG_I5mLKGZGxKnmlzJih40Vx714k9S5fRuQL2mBNYLmotvq6QQ9H3fBKExI-HZXVk_4ywEFbcPxmDZW8bxYLJLSr23UwCgdPeTGmO0Qhv6nBOLPTxcTmFznpZzpaGQ67BLYw1urbmxNpn2raES1tCaaprM66Bb55Fo8x4skLOmRMksb05qfZZrFP58gV8H08xMM3qONQSvYr-vG8MywBrZQuZb85QdnSQ1QP5bbnpRuD6Cc8qX8qM66xtBMiJdAAXdfT16VmLJa_YFiCKjnAg0hhhiAnGYoK-rqH8D67yJHPGy3SM9I0ordSOx6N9AoOdlAj0xcCKAL1stnpONO573qwT6mVflEMAVqhvleDTcn8eWIaWseRkTJdVRFo6mtylEfqETeAOvI_wo3xnDKCKIObOYcNEe8PQ7UDpWddhfvOo9UuO_6j20mg49XX7h64O1pF1EgCLaBJvrJ2shz2KiXF6KPsztXsmfuYh54ZobP7KHsRjay8atOsoW8IaqsxEhBRR8OthK7CihtXco2zYGTlik9HJcgrvXGiWjJKlmiCJw9y2XambHGCUfG1T77N-BQav5bSUhQ

POST: http://localhost:5012/api/Patient/register

{
    "DateOfBirth": "01-01-2000",
    "Gender": "Male",
    "Name": "Tomás Leite",
    "Email": "left4deadgame2@gmail.com",
    "Phone": "123-456-7891",
    "Address": "123 Main St, Anytown, USA",
    "Conditions": ["Diabetes", "Hypertension"],
    "EmergencyContact": "Jane Doe: 098-765-4321"
}

*/ 

pm.test("Status code is 201", function () {
    pm.expect(pm.response.code).to.equal(201);
});

pm.test("Response time is less than 2 seconds", function () {
    pm.expect(pm.response.responseTime).to.be.below(2000);
});

pm.test("All values in the response match the expected values", function () {
    const jsonData = pm.response.json();
    
    pm.expect(jsonData.dateOfBirth).to.equal("01-01-2000");
    pm.expect(jsonData.gender).to.equal("Male");
    pm.expect(jsonData.name).to.equal("Tomás Leite");
    pm.expect(jsonData.email).to.equal("left4deadgame2@gmail.com");
    pm.expect(jsonData.phone).to.equal("123-456-7891");
    pm.expect(jsonData.address).to.equal("123 Main St, Anytown, USA");
    
    // Stringify arrays for accurate comparison of Conditions
    const expectedConditions = ["Diabetes", "Hypertension"];
    pm.expect(JSON.stringify(jsonData.conditions)).to.equal(JSON.stringify(expectedConditions));
    
    pm.expect(jsonData.emergencyContact).to.equal("Jane Doe: 098-765-4321");
});
