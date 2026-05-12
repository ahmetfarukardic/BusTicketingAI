export interface Bus {
    id: number;
    plateNumber: string;
    seatCapacity: number;
    companyId: number;
}

export interface CreateTripRequest {
    busId: number;
    originTerminalId: number;
    destinationTerminalId: number;
    departureTime: string;
    basePrice: number;
    estimatedDuration: number;
}

export interface UpdateTripTimeRequest {
    newDepartureTime: string;
}