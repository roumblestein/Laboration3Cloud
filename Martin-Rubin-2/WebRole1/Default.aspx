<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebRole1._Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body style="height: 435px">
    <form id="form1" runat="server">
        <div style="height: 407px">
            <div>
            <h1>&nbsp;&nbsp;&nbsp;  Welcome to CharterResor!</h1>
            <br />
            <br />
             &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp&nbsp&nbsp
            <asp:Button ID="BtnPost" runat="server" Text="Book Flight" Width="182px" Height="45px" OnClick="BtnPost_Click" style="margin-top: 0px"></asp:Button>
            <br />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp&nbsp&nbsp
            <asp:CheckBox ID="HotelCheck" runat="server" Text="Hotel Reservation"></asp:CheckBox>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp&nbsp&nbsp
         
            </div>
    </form>
</body>
</html>