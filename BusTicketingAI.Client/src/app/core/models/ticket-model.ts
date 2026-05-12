export interface TicketDetailDto {
    ticketId: string;
    passengerName: string;
    seatNumber: number;
    price: number;
    status: number;
    departureTerminal: string;
    arrivalTerminal: string;
    departureTime: string;
}

export interface PassengerTicketResponseDto {
    activeTickets: TicketDetailDto[];
    pastTickets: TicketDetailDto[];
}