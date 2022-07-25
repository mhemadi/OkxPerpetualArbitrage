# OkxPerpetualArbitrage
This trading app helps you take advantage of funding income on perpetual markets in the OKx exchange by buying the asset on the spot market and selling it on the perpetual market.
## Table of contents
* [Funding Arbitrage](#funding-arbitrage)
* [How to Use](#how-to-use)
* [Api Methods](#api-methods)

## Funding Arbitrage
Funding rates are a feature of perpetual markets. When a rate is positive, every 8 hours buyer have to pay a fee to sellers and vice versa.
By buying an asset in the spot market and going short on it on the perpetual market on an asset that has a positive funding rate, the trader can earn a funding income every 8 hours as long as the position is still open.
This app lets you see current potential positions that can be opened, and once chosen, opens the position for you.
	
## How to Use
* ِDownload/Clone the project
* Create a subaccount on Okx exchange. (Do not use your main account as the app may interfere with your positions)
* Create an api key for your subaccount
* Enter the key in the appsettings.json
* Make sure the database folder exists and run migrations using update-database
* Run the webapi app and consume the api using a client of your choice or use the BlazorUIExample project
	
## Api Methods
Swagger in included in the webapi project. But here a list of all the endpoints:

* #### api/currentposition
  Gets a list of all the positions that the app is currently in.
* #### api/currentpositioncheckerror
  Gets a list of all the positions that the app is currently in and also checks for possible errors in the data.
* #### api/currentposition/{symbol}/closedata
  Gets data regarding the position that the user wants to close.
* #### api/currentposition/{symbol}/close
  Posts a request to close a position.
* #### api/currentposition/{symbol}/reset
  Posts a request to reset a position, effectively force closing the position on the exchange and deleting records from the database.
  
* #### api/positiondemand/{symbol}/status
  Gets the status of an ongoing open or close request.
* #### api/positiondemand/{demandId}/cancel
  Posts a request to cancel an ongoing open or close request.
  
* #### api/potentialposition
  Gets a list of all potential positions that may be opened.
* #### api/potentialposition/symbol
  Gets a list of string containing all the symbols.
* #### api/potentialposition/{symbol}/openData
  Gets data regarding the position that the user wants to open.
 * #### api/potentialposition/{symbol}/open
    Posts a request to open a position.
