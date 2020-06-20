<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Report.aspx.cs" Inherits="WebRole1.Report" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="IterationText" runat="server"></asp:Label>

            <asp:Label ID="specificholder" runat="server"></asp:Label>

            <asp:GridView ID="GridFlights" runat="server" AutoGenerateColumns="False" DataSourceID="Flights" >
                <Columns>
                   <asp:BoundField DataField="Id" HeaderText="flightId" />
                    <asp:BoundField DataField="passenger" HeaderText="passenger"  />
                    <asp:BoundField DataField="date" HeaderText="Date"  />
                    <asp:BoundField DataField="price" HeaderText="Price"  />
                    <asp:BoundField DataField="departurecity" HeaderText="DepartureCity"  />
                    <asp:BoundField DataField="arrivalCity" HeaderText="ArrivalCity"  />
                </Columns>
            </asp:GridView>
            <asp:SqlDataSource ID="Flights" runat="server" ConnectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=newdatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False" SelectCommand="SELECT * FROM [Flights] WHERE Id = '1003'"></asp:SqlDataSource>

              <asp:GridView ID="GridAirlines" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSource3" >
                <Columns>
                   <asp:BoundField DataField="code" HeaderText="AirlineCode"  />
                    <asp:BoundField DataField="name" HeaderText="AirlineName"  />
                    
                </Columns>
            </asp:GridView>
            <asp:SqlDataSource ID="SqlDataSource3" runat="server" ConnectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=newdatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False" SelectCommand="SELECT * FROM [Airlines]"></asp:SqlDataSource>


            <asp:GridView ID="GridAirports" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSource1" >
                <Columns>
                   <asp:BoundField DataField="code" HeaderText="Airports"  />
                    <asp:BoundField DataField="city" HeaderText="City"  />
                    <asp:BoundField DataField="latitude" HeaderText="Laititude"  />
                    <asp:BoundField DataField="lonitude" HeaderText="Longitude"  />
                </Columns>
            </asp:GridView>
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=newdatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False" SelectCommand="SELECT * FROM [Airports]"></asp:SqlDataSource>

            <asp:GridView ID="GridRoutes" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSource2" >
                <Columns>
                   <asp:BoundField DataField="flightnumber" HeaderText="FlightId"  />
                    <asp:BoundField DataField="carrier" HeaderText="AirlineCarrier"  />
                    <asp:BoundField DataField="departure" HeaderText="DepartureAirport"  />
                    <asp:BoundField DataField="arrival" HeaderText="ArrivalAirport"  />
                </Columns>
            </asp:GridView>
            <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=newdatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False" SelectCommand="SELECT * FROM [Routes]"></asp:SqlDataSource>


             
            

        </div>
    </form>
</body>
</html>
