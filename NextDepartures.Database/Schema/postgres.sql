drop table if exists gtfs_agency;

create table gtfs_agency (
    agency_id character varying(255),
    agency_name character varying(255) not null,
    agency_url character varying(255) not null,
    agency_timezone character varying(255) not null,
    agency_lang character varying(255),
    agency_phone character varying(255),
    agency_fare_url character varying(255),
    agency_email character varying(255));

drop table if exists gtfs_calendar;

create table gtfs_calendar (
    service_id character varying(255) primary key,
    monday smallint not null,
    tuesday smallint not null,
    wednesday smallint not null,
    thursday smallint not null,
    friday smallint not null,
    saturday smallint not null,
    sunday smallint not null,
    start_date date not null,
    end_date date not null);

drop table if exists gtfs_calendar_dates;

create table gtfs_calendar_dates (
    service_id character varying(255) not null,
    exception_date date not null,
    exception_type smallint not null);

drop table if exists gtfs_fare_attributes;

create table gtfs_fare_attributes (
    fare_id character varying(255) not null,
    price real not null,
    currency_type character varying(255) not null,
    payment_method smallint not null,
    transfers smallint not null,
    agency_id character varying(255),
    transfer_duration smallint);

drop table if exists gtfs_fare_rules;

create table gtfs_fare_rules (
    fare_id character varying(255) not null,
    route_id character varying(255),
    origin_id character varying(255),
    destination_id character varying(255),
    contains_id character varying(255));

drop table if exists gtfs_frequencies;

create table gtfs_frequencies (
    trip_id character varying(255) not null,
    start_time character varying(255) not null,
    end_time character varying(255) not null,
    headway_secs smallint not null,
    exact_times smallint);

drop table if exists gtfs_levels;

create table gtfs_levels (
    level_id character varying(255) not null,
    level_index real not null,
    level_name character varying(255));

drop table if exists gtfs_pathways;

create table gtfs_pathways (
    pathway_id character varying(255) not null,
    from_stop_id character varying(255) not null,
    to_stop_id character varying(255) not null,
    pathway_mode smallint not null,
    is_bidirectional smallint not null,
    length real,
    traversal_time smallint,
    stair_count smallint,
    max_slope real,
    min_width real,
    signposted_as character varying(255),
    reversed_signposted_as character varying(255));

drop table if exists gtfs_routes;

create table gtfs_routes (
    route_id character varying(255) primary key,
    agency_id character varying(255),
    route_short_name character varying(255),
    route_long_name character varying(255),
    route_desc character varying(255),
    route_type smallint not null,
    route_url character varying(255),
    route_color smallint,
    route_text_color smallint,
    route_sort_order smallint);

drop table if exists gtfs_shapes;

create table gtfs_shapes (
    shape_id character varying(255) not null,
    shape_pt_lat real not null,
    shape_pt_lon real not null,
    shape_pt_sequence smallint not null,
    shape_distance_travelled real);

drop table if exists gtfs_stops;

create table gtfs_stops (
    stop_id character varying(255) primary key,
    stop_code character varying(255),
    stop_name character varying(255),
    stop_desc character varying(255),
    stop_lat real,
    stop_lon real,
    zone_id character varying(255),
    stop_url character varying(255),
    location_type smallint,
    parent_station character varying(255),
    stop_timezone character varying(255),
    wheelchair_boarding smallint,
    level_id character varying(255),
    platform_code character varying(255));

drop table if exists gtfs_stop_times;

create table gtfs_stop_times (
    trip_id character varying(255) not null,
    arrival_time character varying(255),
    departure_time character varying(255),
    stop_id character varying(255),
    stop_sequence smallint not null,
    stop_headsign character varying(255),
    pickup_type smallint,
    drop_off_type smallint,
    shape_dist_travelled real,
    timepoint smallint);

drop table if exists gtfs_transfers;

create table gtfs_transfers (
    from_stop_id character varying(255),
    to_stop_id character varying(255),
    transfer_type smallint not null,
    min_transfer_time smallint);

drop table if exists gtfs_trips;

create table gtfs_trips (
    route_id character varying(255) not null,
    service_id character varying(255) not null,
    trip_id character varying(255) primary key,
    trip_headsign character varying(255),
    trip_short_name character varying(255),
    direction_id smallint,
    block_id character varying(255),
    shape_id character varying(255),
    wheelchair_accessible smallint,
    bikes_allowed smallint);