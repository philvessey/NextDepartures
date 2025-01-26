drop table if exists gtfs_agency;

create table gtfs_agency
(
    id character varying(255),
    name character varying(255) not null,
    url character varying(255) not null,
    timezone character varying(255) not null,
    language_code character varying(255),
    phone character varying(255),
    fare_url character varying(255),
    email character varying(255)
);

drop table if exists gtfs_calendar;

create table gtfs_calendar
(
    service_id character varying(255) primary key,
    monday smallint not null,
    tuesday smallint not null,
    wednesday smallint not null,
    thursday smallint not null,
    friday smallint not null,
    saturday smallint not null,
    sunday smallint not null,
    start_date date not null,
    end_date date not null
);

drop table if exists gtfs_calendar_date;

create table gtfs_calendar_date
(
    service_id character varying(255) not null,
    exception_date date not null,
    exception_type smallint not null
);

drop table if exists gtfs_fare_attribute;

create table gtfs_fare_attribute
(
    fare_id character varying(255) not null,
    price real not null,
    currency_type character varying(255) not null,
    payment_method smallint not null,
    transfers smallint not null,
    agency_id character varying(255),
    transfer_duration smallint
);

drop table if exists gtfs_fare_rule;

create table gtfs_fare_rule
(
    fare_id character varying(255) not null,
    route_id character varying(255),
    origin_id character varying(255),
    destination_id character varying(255),
    contains_id character varying(255)
);

drop table if exists gtfs_frequency;

create table gtfs_frequency
(
    trip_id character varying(255) not null,
    start_time character varying(255) not null,
    end_time character varying(255) not null,
    headway_secs smallint not null,
    exact_times smallint
);

drop table if exists gtfs_level;

create table gtfs_level
(
    id character varying(255) not null,
    idx real not null,
    name character varying(255)
);

drop table if exists gtfs_pathway;

create table gtfs_pathway
(
    id character varying(255) not null,
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
    reversed_signposted_as character varying(255)
);

drop table if exists gtfs_route;

create table gtfs_route
(
    id character varying(255) primary key,
    agency_id character varying(255),
    short_name character varying(255),
    long_name character varying(255),
    description character varying(255),
    type smallint not null,
    url character varying(255),
    color smallint,
    text_color smallint
);

drop table if exists gtfs_shape;

create table gtfs_shape
(
    id character varying(255) not null,
    longitude real not null,
    latitude real not null,
    sequence smallint not null,
    distance_travelled real
);

drop table if exists gtfs_stop;

create table gtfs_stop
(
    id character varying(255) primary key,
    code character varying(255),
    name character varying(255),
    description character varying(255),
    longitude real,
    latitude real,
    zone character varying(255),
    url character varying(255),
    location_type smallint,
    parent_station character varying(255),
    timezone character varying(255),
    wheelchair_boarding smallint,
    level_id character varying(255),
    platform_code character varying(255)
);

drop table if exists gtfs_stop_time;

create table gtfs_stop_time
(
    trip_id character varying(255) not null,
    arrival_time character varying(255),
    departure_time character varying(255),
    stop_id character varying(255),
    stop_sequence smallint not null,
    stop_headsign character varying(255),
    pickup_type smallint,
    drop_off_type smallint,
    shape_dist_travelled real,
    timepoint_type smallint
);

drop table if exists gtfs_transfer;

create table gtfs_transfer
(
    from_stop_id character varying(255),
    to_stop_id character varying(255),
    transfer_type smallint not null,
    minimum_transfer_time smallint
);

drop table if exists gtfs_trip;

create table gtfs_trip
(
    id character varying(255) primary key,
    route_id character varying(255) not null,
    service_id character varying(255) not null,
    headsign character varying(255),
    short_name character varying(255),
    direction smallint,
    block_id character varying(255),
    shape_id character varying(255),
    accessibility_type smallint
);