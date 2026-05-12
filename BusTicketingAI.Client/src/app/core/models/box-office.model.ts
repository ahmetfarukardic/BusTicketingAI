export interface CompanyActiveTrip {
    tripId: string;
    originTerminal: string;
    destinationTerminal: string;
    departureTime: string;
    busPlate: string;
    price: number;
    totalSeats: number;
}

export interface SellTicketRequest {
    tripId: string;
    seatNumber: number;
    passengerName: string;
    passengerTC: string;
    price: number;
}

export interface Terminal {
    id: number;
    name: string;
    cityName: string;
}